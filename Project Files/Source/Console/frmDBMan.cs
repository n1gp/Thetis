﻿/*  frmDBMan.cs

This file is part of a program that implements a Software-Defined Radio.

This code/file can be found on GitHub : https://github.com/ramdor/Thetis

Copyright (C) 2020-2024 Richard Samphire MW0LGE

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

The author can be reached by email at

mw0lge@grange-lane.co.uk
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Thetis
{
    public partial class frmDBMan : Form
    {
        private bool _allow_check_change;
        private bool _ignore_lstActiveDBs_selectectedindexchanged;

        public frmDBMan()
        {
            _allow_check_change = true;
            _ignore_lstActiveDBs_selectectedindexchanged = false;
            InitializeComponent();
        }
        public void Restore()
        {
            Common.RestoreForm(this, "DBManForm", true);
        }
        internal void InitBackups(List<DBMan.BackupFileInfo> backups)
        {
            lstBackups.Items.Clear();
            foreach(DBMan.BackupFileInfo backup in backups)
            {
                ListViewItem lvi = new ListViewItem(backup.DateTimeOfBackup.ToString("G"));

                TimeSpan difference = DateTime.Now - backup.DateTimeOfBackup;
                string age;
                if (difference.Days > 0)
                    age = $"{difference.Days}d {difference.Hours.ToString("00")}:{difference.Minutes.ToString("00")}:{difference.Seconds.ToString("00")}";
                else
                    age = $"{difference.Hours.ToString("00")}:{difference.Minutes.ToString("00")}:{difference.Seconds.ToString("00")}";
                lvi.SubItems.Add(age);
                lvi.SubItems.Add(backup.FullFilePath);

                lvi.Tag = backup.FullFilePath;

                lstBackups.Items.Add(lvi);
            }
            lstBackups.ColumnWidthChanging -= lstBackups_ColumnWidthChanging;
            foreach (ColumnHeader column in lstBackups.Columns)
                column.Width = -2;
            lstBackups.ColumnWidthChanging += lstBackups_ColumnWidthChanging;

            lstBackups.Enabled = lstBackups.Items.Count > 0;

            lstBackups_SelectedIndexChanged(this, EventArgs.Empty);
        }
        internal void InitAvailableDBs(Dictionary<Guid, DBMan.DatabaseInfo> dbs, Guid active_guid, Guid reselect_guid)
        {
            bool old_allow_check = _allow_check_change;
            _allow_check_change = true;

            _ignore_lstActiveDBs_selectectedindexchanged = true;

            lstActiveDBs.Items.Clear();
            foreach (KeyValuePair<Guid, DBMan.DatabaseInfo> kvp in dbs)
            {
                DBMan.DatabaseInfo dbi = kvp.Value;

                ListViewItem lvi = new ListViewItem(dbi.Description);

                lvi.Checked = dbi.GUID == active_guid;

                lvi.SubItems.Add(dbi.Model.ToString());
                lvi.SubItems.Add(dbi.LastChanged.ToString("G"));

                TimeSpan difference = DateTime.Now - dbi.CreationTime;
                string age;
                if(difference.Days > 0)
                    age = $"{difference.Days}d {difference.Hours.ToString("00")}:{difference.Minutes.ToString("00")}:{difference.Seconds.ToString("00")}";
                else
                    age = $"{difference.Hours.ToString("00")}:{difference.Minutes.ToString("00")}:{difference.Seconds.ToString("00")}";

                lvi.SubItems.Add(age);
                lvi.SubItems.Add(getReadableFileSize(dbi.Size));
                lvi.SubItems.Add(dbi.BackupOnStartup ? "yes" : "no");
                lvi.SubItems.Add(dbi.BackupOnShutdown ? "yes" : "no");
                lvi.SubItems.Add(dbi.VersionNumber);
                lvi.SubItems.Add(dbi.FullPath);

                lvi.Tag = dbi.GUID.ToString();

                if (reselect_guid != Guid.Empty && dbi.GUID == reselect_guid) lvi.Selected = true;

                lstActiveDBs.Items.Add(lvi);                
            }

            lstActiveDBs.ColumnWidthChanging -= lstActiveDBs_ColumnWidthChanging;
            foreach (ColumnHeader column in lstActiveDBs.Columns)
                column.Width = -2;
            lstActiveDBs.ColumnWidthChanging += lstActiveDBs_ColumnWidthChanging;

            _ignore_lstActiveDBs_selectectedindexchanged = false;

            lstActiveDBs_SelectedIndexChanged(this, EventArgs.Empty);
            _allow_check_change = old_allow_check;
        }

        private string getReadableFileSize(long byteSize)
        {
            if (byteSize >= 1024 * 1024)
            {
                return string.Format("{0:0.00}MB", byteSize / (1024.0 * 1024.0));
            }
            else if (byteSize >= 1024)
            {
                return string.Format("{0:0.00}KB", byteSize / 1024.0);
            }
            else
            {
                return string.Format("{0}B", byteSize);
            }
        }

        private void lstActiveDBs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // prevent user from clicking the checkbox
            if (_allow_check_change) return;

            e.NewValue = e.CurrentValue;
        }

        private void frmDBMan_Shown(object sender, EventArgs e)
        {
            _allow_check_change = false;
        }

        private void frmDBMan_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Common.SaveForm(this, "DBManForm");

                _allow_check_change = true;
            }
        }

        private void btnMakeActive_Click(object sender, EventArgs e)
        {
            if (lstActiveDBs.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstActiveDBs.SelectedItems[0];

            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.MakeActiveDB(guid);
        }

        private void btnNewDB_Click(object sender, EventArgs e)
        {
            DBMan.NewDB();
        }

        private void btnDuplicateBD_Click(object sender, EventArgs e)
        {
            if (lstActiveDBs.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstActiveDBs.SelectedItems[0];

            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.DuplicateDB(guid);
        }

        private void btnRemoveDB_Click(object sender, EventArgs e)
        {
            if (lstActiveDBs.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstActiveDBs.SelectedItems[0];

            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.RemoveDB(guid);
        }        
        private void lstActiveDBs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ignore_lstActiveDBs_selectectedindexchanged) return;

            bool enabled = lstActiveDBs.SelectedItems.Count != 0;

            btnNewDB.Enabled = true;
            btnTakeBackupNow.Enabled = true;
            btnImport.Enabled = true;

            btnRename.Enabled = enabled;
            btnExport.Enabled = enabled;
            btnDuplicateDB.Enabled = enabled;
            btnBackupOnStart.Enabled = enabled;
            btnBackupOnShutdown.Enabled = enabled;

            //force disable if on current active
            if (lstActiveDBs.SelectedItems.Count == 1 && lstActiveDBs.SelectedItems[0].Checked)
                enabled = false;

            btnMakeActive.Enabled = enabled;
            btnRemoveDB.Enabled = enabled;

            Guid selected = Guid.Empty;
            if (lstActiveDBs.SelectedItems.Count == 1)
            {
                ListViewItem lvi = lstActiveDBs.SelectedItems[0];
                selected = new Guid(lvi.Tag.ToString());
            }

            DBMan.SelectedAvailable(selected);
        }

        private void btnTakeBackupNow_Click(object sender, EventArgs e)
        {
            Guid highlighted = Guid.Empty;
            if (lstActiveDBs.SelectedItems.Count == 1)
            {
                ListViewItem lvi = lstActiveDBs.SelectedItems[0];
                highlighted = new Guid(lvi.Tag.ToString());
            }
            //send highlight guid so that we can refresh the lst if it is our active db, as the backup is for the active
            DBMan.TakeBackup(highlighted);
        }

        private void lstBackups_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnMakeBackupAvailable.Enabled = lstBackups.SelectedItems.Count == 1;
            btnExportBackup.Enabled = lstBackups.SelectedItems.Count == 1;

            btnRemoveBackup.Enabled = lstBackups.SelectedItems.Count > 0;
        }

        private void btnMakeBackupAvailable_Click(object sender, EventArgs e)
        {
            if (lstBackups.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstBackups.SelectedItems[0];

            string file_path = lvi.Tag.ToString();

            DBMan.MakeBackupAvailable(file_path);
        }

        private void btnRemoveBackup_Click(object sender, EventArgs e)
        {
            if (lstBackups.SelectedItems.Count < 1) return;

            List<string> file_paths = new List<string>();
            foreach(ListViewItem item in lstBackups.SelectedItems)
            {
                file_paths.Add(item.Tag.ToString());
            }
            DBMan.RemoveBackupDB(file_paths);

            if (lstActiveDBs.SelectedItems.Count != 1) return;
            ListViewItem lvi = lstActiveDBs.SelectedItems[0];
            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.SelectedAvailable(guid);
        }

        //
        private void lstActiveDBs_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            // Cancel the event to prevent the column from being resized
            e.NewWidth = lstActiveDBs.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }
        private void lstBackups_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            // Cancel the event to prevent the column from being resized
            e.NewWidth = lstBackups.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        private void btnBackupOnStart_Click(object sender, EventArgs e)
        {
            if (lstActiveDBs.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstActiveDBs.SelectedItems[0];

            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.BackupOnStartUpToggle(guid);
        }

        private void btnBackupOnShutdown_Click(object sender, EventArgs e)
        {
            if (lstActiveDBs.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstActiveDBs.SelectedItems[0];

            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.BackupOnShutDownToggle(guid);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Guid highlighted = Guid.Empty;
            if (lstActiveDBs.SelectedItems.Count == 1)
            {
                ListViewItem lvi = lstActiveDBs.SelectedItems[0];
                highlighted = new Guid(lvi.Tag.ToString());
            }
            //send highlight guid so that we can refresh the lst if it is our active db, as the backup is for the active
            DBMan.Import();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lstActiveDBs.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstActiveDBs.SelectedItems[0];

            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.Export(guid);
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (lstActiveDBs.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstActiveDBs.SelectedItems[0];

            Guid guid = new Guid(lvi.Tag.ToString());

            DBMan.Rename(guid);
        }

        private void btnExportBackup_Click(object sender, EventArgs e)
        {
            if (lstBackups.SelectedItems.Count != 1) return;

            ListViewItem lvi = lstBackups.SelectedItems[0];

            DBMan.ExportBackup(lvi.Tag.ToString());
        }
    }
}