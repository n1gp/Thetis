/*  ucOtherButtonsOptionsGrid.cs

This file is part of a program that implements a Software-Defined Radio.

This code/file can be found on GitHub : https://github.com/ramdor/Thetis

Copyright (C) 2020-2025 Richard Samphire MW0LGE

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
//
//============================================================================================//
// Dual-Licensing Statement (Applies Only to Author's Contributions, Richard Samphire MW0LGE) //
// ------------------------------------------------------------------------------------------ //
// For any code originally written by Richard Samphire MW0LGE, or for any modifications       //
// made by him, the copyright holder for those portions (Richard Samphire) reserves the       //
// right to use, license, and distribute such code under different terms, including           //
// closed-source and proprietary licences, in addition to the GNU General Public License      //
// granted above. Nothing in this statement restricts any rights granted to recipients under  //
// the GNU GPL. Code contributed by others (not Richard Samphire) remains licensed under      //
// its original terms and is not affected by this dual-licensing statement in any way.        //
// Richard Samphire can be reached by email at :  mw0lge@grange-lane.co.uk                    //
//============================================================================================//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Thetis
{
    public enum OtherButtonId
    {
        POWER = 0,
        RX_2 = 1,
        MON = 2,
        TUN = 3,
        MOX = 4,
        TWOTON = 5,
        DUP = 6,
        PS_A = 7,
        XPA = 8,
        REC = 9,
        PLAY = 10,
        NR = 11,
        ANF = 12,
        NB = 13,
        SNB = 14,
        MNF = 15,
        MNF_PLUS = 16,
        SPLT = 17,
        A_TO_B = 18,
        ZERO_BEAT = 19,
        B_TO_A = 20,
        IF_TO_V = 21,
        SWAP_AB = 22,
        AVG = 23,
        PEAK = 24,
        CTUN = 25,
        VAC1 = 26,
        VAC2 = 27,
        MUTE = 28,
        BIN = 29,
        SUBRX = 30,
        PAN_SWAP = 31,
        NR1 = 32,
        NR2 = 33,
        NR3 = 34,
        NR4 = 35,
        NB1 = 36,
        NB2 = 37,
        SPLITTER = 999,
        UNKNOWN = 1000
    }

    public static class OtherButtonIdHelpers
    {
        public static readonly (OtherButtonId id, int bit_group, int bit_number)[] CheckBoxData =
        new (OtherButtonId, int, int)[]
        {
            (OtherButtonId.POWER,     0,  0),
            (OtherButtonId.RX_2,      0,  1),
            (OtherButtonId.MON,       0,  2),
            (OtherButtonId.TUN,       0,  3),
            (OtherButtonId.MOX,       0,  4),
            (OtherButtonId.TWOTON,    0,  5),
            (OtherButtonId.DUP,       0,  6),
            (OtherButtonId.PS_A,      0,  7),
            (OtherButtonId.XPA,       0,  8),
            (OtherButtonId.REC,       0,  9),
            (OtherButtonId.PLAY,      0, 10),
            (OtherButtonId.SPLITTER, -1, -1),
            (OtherButtonId.NR,        0, 11),
            (OtherButtonId.NR1,       0, 12),
            (OtherButtonId.NR2,       0, 13),
            (OtherButtonId.NR3,       0, 14),
            (OtherButtonId.NR4,       0, 15),
            (OtherButtonId.ANF,       0, 16),
            (OtherButtonId.NB,        0, 17),
            (OtherButtonId.NB1,       0, 18),
            (OtherButtonId.NB2,       0, 19),
            (OtherButtonId.SNB,       0, 20),
            (OtherButtonId.MNF,       0, 21),
            (OtherButtonId.MNF_PLUS,  0, 22),
            (OtherButtonId.SPLITTER, -1, -1),
            (OtherButtonId.SPLT,      0, 23),
            (OtherButtonId.A_TO_B,    0, 24),
            (OtherButtonId.ZERO_BEAT, 0, 25),
            (OtherButtonId.B_TO_A,    0, 26),
            (OtherButtonId.IF_TO_V,   0, 27),
            (OtherButtonId.SWAP_AB,   0, 28),
            (OtherButtonId.SPLITTER, -1, -1),
            (OtherButtonId.AVG,       0, 29),
            (OtherButtonId.PEAK,      0, 30),
            (OtherButtonId.CTUN,      1, 0),
            (OtherButtonId.SPLITTER, -1, -1),
            (OtherButtonId.VAC1,      1, 1),
            (OtherButtonId.VAC2,      1, 2),
            (OtherButtonId.MUTE,      1, 4),
            (OtherButtonId.BIN,       1, 5),
            (OtherButtonId.SUBRX,     1, 6),
            (OtherButtonId.PAN_SWAP,  1, 7),
        };

        public static string OtherButtonIDToText(OtherButtonId id)
        {
            switch (id)
            {
                case OtherButtonId.POWER: return "Power";
                case OtherButtonId.RX_2: return "RX 2";
                case OtherButtonId.MON: return "MON";
                case OtherButtonId.TUN: return "TUN";
                case OtherButtonId.MOX: return "MOX";
                case OtherButtonId.TWOTON: return "2TON";
                case OtherButtonId.DUP: return "DUP";
                case OtherButtonId.PS_A: return "PS-A";
                case OtherButtonId.XPA: return "xPA";
                case OtherButtonId.REC: return "Rec";
                case OtherButtonId.PLAY: return "Play";
                case OtherButtonId.NR: return "NR";
                case OtherButtonId.ANF: return "ANF";
                case OtherButtonId.NB: return "NB";
                case OtherButtonId.SNB: return "SNB";
                case OtherButtonId.MNF: return "MNF";
                case OtherButtonId.MNF_PLUS: return "MNF+";
                case OtherButtonId.SPLT: return "Split";
                case OtherButtonId.A_TO_B: return "A > B";
                case OtherButtonId.ZERO_BEAT: return "0 Beat";
                case OtherButtonId.B_TO_A: return "A < B";
                case OtherButtonId.IF_TO_V: return "IF > V";
                case OtherButtonId.SWAP_AB: return "A <> B";
                case OtherButtonId.AVG: return "Avg";
                case OtherButtonId.PEAK: return "Peak";
                case OtherButtonId.CTUN: return "CTUN";
                case OtherButtonId.VAC1: return "Vac1";
                case OtherButtonId.VAC2: return "Vac2";
                case OtherButtonId.MUTE: return "Mute";
                case OtherButtonId.BIN: return "Bin";
                case OtherButtonId.SUBRX: return "SubRX";
                case OtherButtonId.PAN_SWAP: return "SwapLR";

                case OtherButtonId.NR1: return "NR1";
                case OtherButtonId.NR2: return "NR2";
                case OtherButtonId.NR3: return "NR3";
                case OtherButtonId.NR4: return "NR4";

                case OtherButtonId.NB1: return "NB1";
                case OtherButtonId.NB2: return "NB2";

                case OtherButtonId.UNKNOWN: return "";

                default: return id.ToString();
            }
        }

        public static OtherButtonId BitToID(int bit_group, int bit_number)
        {
            for (int i = 0; i < CheckBoxData.Length; i++)
            {
                if (CheckBoxData[i].bit_group != bit_group) continue;
                if (CheckBoxData[i].bit_number != bit_number) continue;
                return CheckBoxData[i].id;
            }
            return OtherButtonId.UNKNOWN;
        }

        public static (int bit_group, int bit) BitFromID(OtherButtonId id)
        {
            for (int i = 0; i < CheckBoxData.Length; i++)
            {
                if (CheckBoxData[i].id == id) return (CheckBoxData[i].bit_group, CheckBoxData[i].bit_number);
            }
            return (-1, -1);
        }

        public static string BitToText(int bit_group, int bit_number)
        {
            OtherButtonId id = BitToID(bit_group, bit_number);
            return OtherButtonIDToText(id);
        }
    }

    public partial class ucOtherButtonsOptionsGrid : UserControl
    {
        private List<CheckBox> _check_boxes;
        private bool _init;
        public event EventHandler CheckboxChanged;

        private TableLayoutPanel _table;

        public ucOtherButtonsOptionsGrid()
        {
            _init = false;
            InitializeComponent();

            this.Size = new Size(157, 182);
            this.scrollableControl1.Location = new Point(0, 0);
            this.scrollableControl1.Size = new Size(157, 182);
            this.scrollableControl1.AutoScroll = true;

            initialise_checkboxes();
        }

        private void initialise_checkboxes()
        {
            if (_check_boxes == null)
            {
                _check_boxes = new List<CheckBox>();
            }
            else
            {
                for (int i = 0; i < _check_boxes.Count; i++)
                    _check_boxes[i].CheckedChanged -= checkbox_checked_changed;

                _check_boxes.Clear();
            }

            scrollableControl1.Controls.Clear();

            TableLayoutPanel table = new TableLayoutPanel();
            table.Name = "tbl_other_buttons";
            table.AutoSize = true;
            table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            table.Dock = DockStyle.Top;
            table.ColumnCount = 2;
            table.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            scrollableControl1.Controls.Add(table);

            int row = 0;
            int col = 0;
            (OtherButtonId id, int bit_group, int bit_number)[] data = OtherButtonIdHelpers.CheckBoxData;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].id == OtherButtonId.SPLITTER)
                {
                    if (col != 0)
                    {
                        col = 0;
                        row++;
                    }

                    Panel sep = new Panel();
                    sep.Name = "sep_" + i.ToString();
                    sep.Height = 1;
                    sep.Dock = DockStyle.Fill;
                    sep.Margin = new Padding(0, 4, 0, 4);
                    sep.BackColor = SystemColors.ControlDark;

                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    table.Controls.Add(sep, 0, row);
                    table.SetColumnSpan(sep, 2);

                    row++;
                    continue;
                }

                CheckBox chk = new CheckBox();
                chk.Name = "chk_" + ((int)data[i].id).ToString();
                chk.AutoSize = true;
                chk.Margin = new Padding(2, 0, 2, 0);
                chk.Text = OtherButtonIdHelpers.OtherButtonIDToText(data[i].id);
                chk.Tag = new ValueTuple<OtherButtonId, int, int>(data[i].id, data[i].bit_group, data[i].bit_number);
                chk.CheckedChanged += checkbox_checked_changed;

                if (col == 0)
                {
                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    table.RowCount = row + 1;
                }

                table.Controls.Add(chk, col, row);
                _check_boxes.Add(chk);

                col++;
                if (col > 1)
                {
                    col = 0;
                    row++;
                }
            }

            _init = true;
        }

        private void checkbox_checked_changed(object sender, EventArgs e)
        {
            if (!_init) return;
            if (CheckboxChanged != null) CheckboxChanged(this, EventArgs.Empty);
        }

        private int get_bitfield(int bit_group)
        {
            int bitfield_value = 0;

            for (int i = 0; i < _check_boxes.Count; i++)
            {
                CheckBox checkbox = _check_boxes[i];
                ValueTuple<OtherButtonId, int, int> meta = (ValueTuple<OtherButtonId, int, int>)checkbox.Tag;
                if (meta.Item2 != bit_group) continue;
                if (meta.Item3 < 0) continue;
                if (checkbox.Checked) bitfield_value |= (1 << meta.Item3);
            }

            return bitfield_value;
        }

        private void set_bitfield(int bit_group, int value)
        {
            bool old_init = _init;
            _init = false;

            for (int i = 0; i < _check_boxes.Count; i++)
            {
                CheckBox checkbox = _check_boxes[i];
                ValueTuple<OtherButtonId, int, int> meta = (ValueTuple<OtherButtonId, int, int>)checkbox.Tag;
                if (meta.Item2 != bit_group) continue;
                if (meta.Item3 < 0) continue;
                bool is_checked = (value & (1 << meta.Item3)) != 0;
                checkbox.Checked = is_checked;
            }

            _init = old_init;
        }

        //public int BitfieldGroup0
        //{
        //    get { return get_bitfield(0); }
        //    set { set_bitfield(0, value); }
        //}

        //public int BitfieldGroup1
        //{
        //    get { return get_bitfield(1); }
        //    set { set_bitfield(1, value); }
        //}

        public int GetBitfield(int bit_group)
        {
            return get_bitfield(bit_group);
        }
        public void SetBitfield(int bit_group, int value)
        {
            set_bitfield(bit_group, value);
        }

        public int GetCheckedCount()
        {
            int count = 0;
            for (int i = 0; i < _check_boxes.Count; i++)
            {
                CheckBox checkbox = _check_boxes[i];
                if (checkbox.Checked) count++;
            }
            return count;
        }

        public int GetCheckedCount(int bit_group)
        {
            int count = 0;
            for (int i = 0; i < _check_boxes.Count; i++)
            {
                CheckBox checkbox = _check_boxes[i];
                ValueTuple<OtherButtonId, int, int> meta = (ValueTuple<OtherButtonId, int, int>)checkbox.Tag;
                if (meta.Item2 != bit_group) continue;
                if (meta.Item3 < 0) continue;
                if (checkbox.Checked) count++;
            }
            return count;
        }
    }
}

