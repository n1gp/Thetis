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
        RX_2,
        MON,
        TUN,
        MOX,
        TWOTON,
        DUP,
        PS_A,
        XPA,
        REC,
        PLAY,
        NR,
        ANF,
        NB,
        SNB,
        MNF,
        MNF_PLUS,
        SPLT,
        A_TO_B,
        ZERO_BEAT,
        B_TO_A,
        IF_TO_V,
        SWAP_AB,
        AVG,
        PEAK,
        CTUN,
        VAC1,
        VAC2,
        MUTE,
        BIN,
        SUBRX,
        PAN_SWAP,
        NR1,
        NR2,
        NR3,
        NR4,
        NB1,
        NB2,
        SPECTRUM,
        PANADAPTER,
        SCOPE,
        SCOPE2,
        PHASE,
        WATERFALL,
        HISTOGRAM,
        PANAFALL,
        PANASCOPE,
        SPECTRASCOPE,
        DISPLAY_OFF,
        PAUSE,
        PEAK_BLOBS,
        CURSOR_INFO,
        SPOTS,
        FILL_SPECTRUM,
        SQL,
        SQL_SQL,
        SQL_VSQL,
        RIT,
        RIT0,
        XIT,
        XIT0,
        MIC,
        COMP,
        VOX,
        DEXP,
        RX_EQ,
        TX_EQ,
        TX_FILTER,
        CFC,
        CFC_EQ,
        LEVELER,
        AGC_FIXED,
        AGC_LONG,
        AGC_SLOW,
        AGC_MEDIUM,
        AGC_FAST,
        AGC_CUSTOM,
        AGC_AUTO,
        DITHER,
        RANDOM,
        SR_48000,
        SR_96000,
        SR_192000,
        SR_384000,
        SR_768000,
        SR_1536000,
        ATT_STEP,
        ATT_0,
        ATT_10,
        ATT_20,
        ATT_30,
        ATT_40,
        ATT_50,
        ATT_P1,
        ATT_M1,
        PAN_P5,
        PAN_M5,
        PAN_CENTRE,
        ZTB,
        ZOOM_0P5,
        ZOOM_1,
        ZOOM_2,
        ZOOM_4,
        MUTE_ALL,
        MAF_P5,
        MAF_M5,
        AF_P5,
        AF_M5,
        BAL_P5,
        BAL_M5,
        SAF_P5,
        SAF_M5,
        SBAL_P5,
        SBAL_M5,
        SQL_P5,
        SQL_M5,
        MIC_P1,
        MIC_M1,
        COMP_P1,
        COMP_M1,
        VOX_P1,
        VOX_M1,
        AGC_P5,
        AGC_M5,
        DRIVE_P5,
        DRIVE_M5,
        TUNE_P5,
        TUNE_M5,
        DRIVE_0,
        TUN_0,
        FORM_SETUP,
        FORM_DBMAN,
        FORM_MEMORY,
        FORM_WAVE,
        FORM_EQ,
        FORM_XVTR,
        FORM_CWX,
        FORM_DIVERSITY,
        FORM_LINEARITY,
        FORM_WB,
        CWX_KEY,
        CWX_STOP,
        CWX_F1,
        CWX_F2,
        CWX_F3,
        CWX_F4,
        CWX_F5,
        CWX_F6,
        CWX_F7,
        CWX_F8,
        CWX_F9,
        VFO_SYNC,
        LOCK_A,
        LOCK_B,
        TUNE_STEP_U,
        TUNE_STEP_D,
        STACK_U,
        STACK_D,

        INFO_TEXT = 998,
        SPLITTER = 999,

        UNKNOWN = 1000
    }
    
    public static class OtherButtonIdHelpers
    {
        public const int MAX_BITFIELD_GROUP = 11; // the last betfield group + 1 in CheckBoxData

        private static Dictionary<OtherButtonId, int> _id_to_index_map = new Dictionary<OtherButtonId, int>();
        private static Dictionary<(int, int), int> _bit_group_bit_to_index_map = new Dictionary<(int, int), int>();

        public static readonly (OtherButtonId id, int bit_group, int bit_number, string caption, string icon_on, string icon_off)[] CheckBoxData =
        new (OtherButtonId, int, int, string, string, string)[]
        {
            (OtherButtonId.INFO_TEXT, -1, -1, "General", "", ""),
            (OtherButtonId.SPLITTER,  -1, -1, "", "", ""),
            (OtherButtonId.POWER,     0,  0, "Power", "power", ""),
            (OtherButtonId.RX_2,      0,  1, "RX 2", "", ""),
            (OtherButtonId.MON,       0,  2, "MON", "", ""),
            (OtherButtonId.TUN,       0,  3, "TUN", "", ""),
            (OtherButtonId.MOX,       0,  4, "MOX", "", ""),
            (OtherButtonId.TWOTON,    0,  5, "2TON", "", ""),
            (OtherButtonId.DUP,       0,  6, "DUP", "", ""),
            (OtherButtonId.PS_A,      0,  7, "PS-A", "", ""),
            (OtherButtonId.XPA,       0,  8, "xPA", "", ""),
            (OtherButtonId.REC,       0,  9, "Rec", "record", ""),
            (OtherButtonId.PLAY,      0, 10, "Play", "play", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "Noise / ATT", "", ""),
            (OtherButtonId.SPLITTER, -1, -1, "", "", ""),
            (OtherButtonId.NR,        1, 0, "NR", "", ""),
            (OtherButtonId.NR1,       1, 1, "NR1", "", ""),
            (OtherButtonId.NR2,       1, 2, "NR2", "", ""),
            (OtherButtonId.NR3,       1, 3, "NR3", "", ""),
            (OtherButtonId.NR4,       1, 4, "NR4", "", ""),
            (OtherButtonId.ANF,       1, 5, "ANF", "", ""),
            (OtherButtonId.NB,        1, 6, "NB", "", ""),
            (OtherButtonId.NB1,       1, 7, "NB1", "", ""),
            (OtherButtonId.NB2,       1, 8, "NB2", "", ""),
            (OtherButtonId.SNB,       1, 9, "SNB", "", ""),
            (OtherButtonId.MNF,       1, 10, "MNF", "", ""),
            (OtherButtonId.ATT_STEP,  1, 11, "S-ATT", "", ""),
            (OtherButtonId.ATT_0,     1, 12, "ATT-0", "", ""),
            (OtherButtonId.ATT_10,    1, 13, "ATT-10", "", ""),
            (OtherButtonId.ATT_20,    1, 14, "ATT-20", "", ""),
            (OtherButtonId.ATT_30,    1, 15, "ATT-30", "", ""),
            (OtherButtonId.ATT_40,    1, 16, "ATT-40", "", ""),
            (OtherButtonId.ATT_50,    1, 17, "ATT-50", "", ""),
            (OtherButtonId.ATT_P1,    1, 18, "ATT+1", "", ""),
            (OtherButtonId.ATT_M1,    1, 19, "ATT-1", "", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "VFOs", "", ""),
            (OtherButtonId.SPLITTER, -1, -1, "", "", ""),
            (OtherButtonId.SPLT,      2, 0, "Split", "", ""),
            (OtherButtonId.A_TO_B,    2, 1, "A > B", "", ""),
            (OtherButtonId.ZERO_BEAT, 2, 2, "0 Beat", "", ""),
            (OtherButtonId.B_TO_A,    2, 3, "A < B", "", ""),
            (OtherButtonId.IF_TO_V,   2, 4, "IF > V", "", ""),
            (OtherButtonId.SWAP_AB,   2, 5, "A <> B", "", ""),
            (OtherButtonId.RIT,       2, 6, "RIT", "", ""),
            (OtherButtonId.RIT0,      2, 7, "RIT0", "", ""),
            (OtherButtonId.XIT,       2, 8, "XIT", "", ""),
            (OtherButtonId.XIT0,      2, 9, "XIT0", "", ""),
            (OtherButtonId.VFO_SYNC,     2, 10, "SYNC", "", ""),
            (OtherButtonId.LOCK_A,       2, 11, "LockA", "", ""),
            (OtherButtonId.LOCK_B,       2, 12, "LockB", "", ""),
            (OtherButtonId.TUNE_STEP_U,  2, 13, "TS+", "", ""),
            (OtherButtonId.TUNE_STEP_D,  2, 14, "TS-", "", ""),
            (OtherButtonId.STACK_U,      2, 15, "STACK+", "", ""),
            (OtherButtonId.STACK_D,      2, 16, "STATCK-", "", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "Display", "", ""),
            (OtherButtonId.SPLITTER,  -1, -1, "", "", ""),
            (OtherButtonId.AVG,           3, 0, "Avg", "", ""),
            (OtherButtonId.PEAK,          3, 1, "Peak", "", ""),
            (OtherButtonId.CTUN,          3, 2, "CTUN", "", ""),
            (OtherButtonId.SPECTRUM,      3, 3, "Spectrum", "spectrum", ""),
            (OtherButtonId.PANADAPTER,    3, 4, "Panadapter", "panadapter", ""),
            (OtherButtonId.SCOPE,         3, 5, "Scope", "scope", ""),
            (OtherButtonId.SCOPE2,        3, 6, "Scope2", "scope2", ""),
            (OtherButtonId.PHASE,         3, 7, "Phase", "phase", ""),
            (OtherButtonId.WATERFALL,     3, 8, "Waterfall", "waterfall", ""),
            (OtherButtonId.HISTOGRAM,     3, 9, "Histogram", "histogram", ""),
            (OtherButtonId.PANAFALL,      3, 10, "Panafall", "panafall", ""),
            (OtherButtonId.PANASCOPE,     3, 11, "Panascope", "panascope", ""),
            (OtherButtonId.SPECTRASCOPE,  3, 12, "Spectrascope", "spectrascope", ""),
            (OtherButtonId.DISPLAY_OFF,   3, 13, "Off", "display_off", ""),
            (OtherButtonId.PAUSE,         3, 14, "Pause", "", ""),
            (OtherButtonId.PEAK_BLOBS,    3, 15, "Peak Blobs", "", ""),
            (OtherButtonId.CURSOR_INFO,   3, 16, "Cur Info", "", ""),
            (OtherButtonId.SPOTS,         3, 17, "Spots", "", ""),
            (OtherButtonId.FILL_SPECTRUM, 3, 18, "Fill", "", ""),
            (OtherButtonId.PAN_P5,        3, 19, "PAN+", "", ""),
            (OtherButtonId.PAN_M5,        3, 20, "PAN-", "", ""),
            (OtherButtonId.PAN_CENTRE,    3, 21, "Centre", "", ""),
            (OtherButtonId.ZTB,           3, 22, "ZTB", "", ""),
            (OtherButtonId.ZOOM_0P5,      3, 23, "0.5x", "", ""),
            (OtherButtonId.ZOOM_1,        3, 24, "1x", "", ""),
            (OtherButtonId.ZOOM_2,        3, 25, "2x", "", ""),
            (OtherButtonId.ZOOM_4,        3, 26, "4x", "", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "Audio / DSP", "", ""),
            (OtherButtonId.SPLITTER, -1, -1, "", "", ""),
            (OtherButtonId.VAC1,      4, 0, "Vac1", "", ""),
            (OtherButtonId.VAC2,      4, 1, "Vac2", "", ""),
            (OtherButtonId.MUTE,      4, 2, "Mute", "mute_on", "mute_off"),
            (OtherButtonId.MUTE_ALL,  4, 3, "Mute All", "mute_all_on", "mute_all_off"),
            (OtherButtonId.BIN,       4, 4, "Bin", "", ""),
            (OtherButtonId.SUBRX,     4, 5, "SubRX", "", ""),
            (OtherButtonId.PAN_SWAP,  4, 6, "SwapLR", "", ""),
            (OtherButtonId.SQL,       4, 7, "SQL", "", ""),
            (OtherButtonId.SQL_SQL,   4, 8, "Reg SQL", "", ""),
            (OtherButtonId.SQL_VSQL,  4, 9, "Voice SQL", "", ""),
            (OtherButtonId.MIC,       4, 10, "MIC", "mic_on", "mic_off"),
            (OtherButtonId.COMP,      4, 11, "COMP", "", ""),
            (OtherButtonId.VOX,       4, 12, "VOX", "", ""),
            (OtherButtonId.DEXP,      4, 13, "DEXP", "", ""),
            (OtherButtonId.RX_EQ,     4, 14, "RX EQ", "", ""),
            (OtherButtonId.TX_EQ,     4, 15, "TX EQ", "", ""),
            (OtherButtonId.TX_FILTER, 4, 16, "TX Filter", "", ""),
            (OtherButtonId.CFC,       4, 17, "CFC", "", ""),
            (OtherButtonId.CFC_EQ,    4, 18, "CFC EQ", "", ""),
            (OtherButtonId.MAF_P5,    4, 19, "MAF+", "", ""),
            (OtherButtonId.MAF_M5,    4, 20, "MAF-", "", ""),
            (OtherButtonId.AF_P5,     4, 21, "AF+", "", ""),
            (OtherButtonId.AF_M5,     4, 22, "AF-", "", ""),
            (OtherButtonId.BAL_P5,    4, 23, "BAL+", "", ""),
            (OtherButtonId.BAL_M5,    4, 24, "BAL-", "", ""),
            (OtherButtonId.SAF_P5,    4, 25, "SAF+", "", ""),
            (OtherButtonId.SAF_M5,    4, 26, "SAF-", "", ""),
            (OtherButtonId.SBAL_P5,   4, 27, "SBAL+", "", ""),
            (OtherButtonId.SBAL_M5,   4, 28, "SBAL-", "", ""),
            (OtherButtonId.SQL_P5,    4, 29, "SQL+", "", ""),
            (OtherButtonId.SQL_M5,    4, 30, "SQL-", "", ""),
            (OtherButtonId.MIC_P1,    5, 0, "MIC+", "", ""),
            (OtherButtonId.MIC_M1,    5, 1, "MIC-", "", ""),
            (OtherButtonId.COMP_P1,   5, 3, "COMP+", "", ""),
            (OtherButtonId.COMP_M1,   5, 4, "COMP-", "", ""),
            (OtherButtonId.VOX_P1,    5, 5, "VOX+", "", ""),
            (OtherButtonId.VOX_M1,    5, 6, "VOX-", "", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "AGC", "", ""),
            (OtherButtonId.SPLITTER, -1, -1, "", "", ""),
            (OtherButtonId.AGC_FIXED,  6, 0, "FIX", "", ""),
            (OtherButtonId.AGC_LONG,   6, 1, "LONG", "", ""),
            (OtherButtonId.AGC_SLOW,   6, 2, "SLOW", "", ""),
            (OtherButtonId.AGC_MEDIUM, 6, 3, "MED", "", ""),
            (OtherButtonId.AGC_FAST,   6, 4, "FAST", "", ""),
            (OtherButtonId.AGC_CUSTOM, 6, 5, "CUST", "", ""),
            (OtherButtonId.AGC_AUTO,   6, 6, "AUTO", "", ""),
            (OtherButtonId.AGC_P5,     6, 7, "AGC+", "", ""),
            (OtherButtonId.AGC_M5,     6, 8, "AGC-", "", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "Hardware", "", ""),
            (OtherButtonId.SPLITTER, -1, -1, "", "", ""),
            (OtherButtonId.DITHER,     7, 0, "Dither", "", ""),
            (OtherButtonId.RANDOM,     7, 1, "Random", "", ""),
            (OtherButtonId.SR_48000,   7, 2, "48k", "", ""),
            (OtherButtonId.SR_96000,   7, 3, "96k", "", ""),
            (OtherButtonId.SR_192000,  7, 4, "192k", "", ""),
            (OtherButtonId.SR_384000,  7, 5, "384k", "", ""),
            (OtherButtonId.SR_768000,  7, 6, "768k", "", ""),
            (OtherButtonId.SR_1536000, 7, 7, "1536k", "", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "Power", "", ""),
            (OtherButtonId.SPLITTER, -1, -1, "", "", ""),
            (OtherButtonId.DRIVE_P5,    8, 0, "DRI+", "", ""),
            (OtherButtonId.DRIVE_M5,    8, 1, "DRI-", "", ""),
            (OtherButtonId.TUNE_P5,     8, 2, "TUN+", "", ""),
            (OtherButtonId.TUNE_M5,     8, 3, "TUN-", "", ""),
            (OtherButtonId.DRIVE_0,     8, 4, "DRI-0", "", ""),
            (OtherButtonId.TUN_0,       8, 5, "TUN-0", "", ""),
            (OtherButtonId.INFO_TEXT, -1, -1, "Forms", "", ""),
            (OtherButtonId.SPLITTER, -1, -1, "", "", ""),
            (OtherButtonId.FORM_SETUP,     9, 0, "Setup", "", ""),
            (OtherButtonId.FORM_DBMAN,     9, 1, "DB Man", "", ""),
            (OtherButtonId.FORM_MEMORY,    9, 2, "Memory", "", ""),
            (OtherButtonId.FORM_WAVE,      9, 3, "Wave", "", ""),
            (OtherButtonId.FORM_EQ,        9, 4, "Equaliser", "", ""),
            (OtherButtonId.FORM_XVTR,      9, 5, "XVTR", "", ""),
            (OtherButtonId.FORM_CWX,       9, 6, "CWX", "", ""),
            (OtherButtonId.FORM_DIVERSITY, 9, 8, "Diversity", "", ""),
            (OtherButtonId.FORM_LINEARITY, 9, 9, "Linearity", "", ""),
            (OtherButtonId.FORM_WB,        9, 10, "Wideband", "", ""),
            (OtherButtonId.INFO_TEXT,     -1, -1, "CWX", "", ""),
            (OtherButtonId.SPLITTER,      -1, -1, "", "", ""),
            (OtherButtonId.CWX_KEY,        10, 0, "KEY", "", ""),
            (OtherButtonId.CWX_STOP,       10, 1, "STOP", "", ""),
            (OtherButtonId.CWX_F1,         10, 2, "F1", "", ""),
            (OtherButtonId.CWX_F2,         10, 3, "F2", "", ""),
            (OtherButtonId.CWX_F3,         10, 4, "F3", "", ""),
            (OtherButtonId.CWX_F4,         10, 5, "F4", "", ""),
            (OtherButtonId.CWX_F5,         10, 6, "F5", "", ""),
            (OtherButtonId.CWX_F6,         10, 7, "F6", "", ""),
            (OtherButtonId.CWX_F7,         10, 8, "F7", "", ""),
            (OtherButtonId.CWX_F8,         10, 9, "F8", "", ""),
            (OtherButtonId.CWX_F9,         10, 10, "F9", "", ""),
        };

        static OtherButtonIdHelpers()
        {
            _id_to_index_map = new Dictionary<OtherButtonId, int>(CheckBoxData.Length);
            _bit_group_bit_to_index_map = new Dictionary<(int, int), int>(CheckBoxData.Length);

            for (int i = 0; i < CheckBoxData.Length; i++)
            {
                (OtherButtonId id, int bit_group, int bit_number, string caption, string icon_on, string icon_off) = CheckBoxData[i];

                if (!_id_to_index_map.ContainsKey(id)) _id_to_index_map[id] = i;

                (int, int) key = (bit_group, bit_number);
                if (!_bit_group_bit_to_index_map.ContainsKey(key)) _bit_group_bit_to_index_map[key] = i;
            }
        }

        public static string OtherButtonIDToText(OtherButtonId id)
        {
            if (_id_to_index_map.ContainsKey(id)) return CheckBoxData[_id_to_index_map[id]].caption;
            return id.ToString();
        }
        public static string OtherButtonIDToIconOn(OtherButtonId id)
        {
            if (_id_to_index_map.ContainsKey(id)) return CheckBoxData[_id_to_index_map[id]].icon_on;
            return string.Empty;
        }
        public static string OtherButtonIDToIconOff(OtherButtonId id)
        {
            if (_id_to_index_map.ContainsKey(id)) return CheckBoxData[_id_to_index_map[id]].icon_off;
            return string.Empty;
        }


        public static OtherButtonId BitToID(int bit_group, int bit_number)
        {
            if (_bit_group_bit_to_index_map.TryGetValue((bit_group, bit_number), out int idx)) return CheckBoxData[idx].id;
            return OtherButtonId.UNKNOWN;
        }

        public static (int bit_group, int bit) BitFromID(OtherButtonId id)
        {
            if (_id_to_index_map.ContainsKey(id)) return (CheckBoxData[_id_to_index_map[id]].bit_group, CheckBoxData[_id_to_index_map[id]].bit_number);
            return (-1, -1);
        }

        public static string BitToText(int bit_group, int bit_number)
        {
            OtherButtonId id = BitToID(bit_group, bit_number);
            return OtherButtonIDToText(id);
        }
        public static (string, string) BitToIcon(int bit_group, int bit_number)
        {
            OtherButtonId id = BitToID(bit_group, bit_number);
            return (OtherButtonIDToIconOn(id), OtherButtonIDToIconOff(id));
        }
    }

    public partial class ucOtherButtonsOptionsGrid : UserControl
    {
        private List<CheckBox> _check_boxes;
        private bool _init;
        public event EventHandler CheckboxChanged;

        private Dictionary<OtherButtonId, CheckBox> _checkbox_by_id;
        private Dictionary<int, List<(int bit, CheckBox cb)>> _by_group;
        private TableLayoutPanel _table;

        public ucOtherButtonsOptionsGrid()
        {
            _init = false;
            InitializeComponent();

            this.Size = new Size(173, 182);
            this.scrollableControl1.Location = new Point(0, 0);
            this.scrollableControl1.Size = new Size(170, 178);
            this.scrollableControl1.AutoScroll = true;

            _check_boxes = new List<CheckBox>();
            _checkbox_by_id = new Dictionary<OtherButtonId, CheckBox>();
            _by_group = new Dictionary<int, List<(int bit, CheckBox cb)>>();

            initialise_checkboxes();
        }

        private void initialise_checkboxes()
        {
            _init = false;

            for (int i = 0; i < _check_boxes.Count; i++)
                _check_boxes[i].CheckedChanged -= checkbox_checked_changed;
            _check_boxes.Clear();
            _checkbox_by_id.Clear();
            _by_group.Clear();

            if (_table == null)
            {
                _table = new TableLayoutPanel();
                _table.Name = "tbl_other_buttons";
                _table.AutoSize = true;
                _table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _table.Dock = DockStyle.Top;
                _table.ColumnCount = 2;
                _table.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                scrollableControl1.Controls.Add(_table);
            }
            else
            {
                _table.SuspendLayout();
                _table.Controls.Clear();
                _table.RowStyles.Clear();
                _table.RowCount = 0;
                _table.ResumeLayout(false);
            }

            this.SuspendLayout();
            scrollableControl1.SuspendLayout();
            _table.SuspendLayout();
            bool was_visible = scrollableControl1.Visible;
            scrollableControl1.Visible = false;

            int row = 0;
            int col = 0;
            (OtherButtonId id, int bit_group, int bit_number, string caption, string icon_on, string icon_off)[] data = OtherButtonIdHelpers.CheckBoxData;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].id == OtherButtonId.INFO_TEXT)
                {
                    if (col != 0)
                    {
                        col = 0;
                        row++;
                    }

                    LabelTS lbl = new LabelTS();
                    lbl.Name = "lbl_" + i.ToString();
                    lbl.AutoSize = true;
                    lbl.Margin = new Padding(0, 2, 0, 0);
                    lbl.Font = new Font(Font, FontStyle.Bold);
                    lbl.Text = data[i].caption;
                    _table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    _table.Controls.Add(lbl, 0, row);
                    _table.SetColumnSpan(lbl, 2);
                    row++;
                    continue;
                }

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
                    sep.Margin = new Padding(0, 2, 0, 4);
                    sep.BackColor = SystemColors.ControlDark;
                    _table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    _table.Controls.Add(sep, 0, row);
                    _table.SetColumnSpan(sep, 2);
                    row++;
                    continue;
                }

                CheckBox chk = new CheckBox();
                chk.Name = "chk_" + ((int)data[i].id).ToString();
                chk.AutoSize = true;
                chk.Margin = new Padding(0, 0, 0, 0);
                chk.Text = OtherButtonIdHelpers.OtherButtonIDToText(data[i].id);
                chk.Tag = new ValueTuple<OtherButtonId, int, int>(data[i].id, data[i].bit_group, data[i].bit_number);
                chk.CheckedChanged += checkbox_checked_changed;

                if (col == 0)
                {
                    _table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    _table.RowCount = row + 1;
                }

                _table.Controls.Add(chk, col, row);
                _check_boxes.Add(chk);
                _checkbox_by_id[data[i].id] = chk;

                if (data[i].bit_group >= 0 && data[i].bit_number >= 0)
                {
                    List<(int bit, CheckBox cb)> list;
                    if (!_by_group.TryGetValue(data[i].bit_group, out list))
                    {
                        list = new List<(int bit, CheckBox cb)>();
                        _by_group[data[i].bit_group] = list;
                    }
                    list.Add((data[i].bit_number, chk));
                }

                col++;
                if (col > 1)
                {
                    col = 0;
                    row++;
                }
            }

            scrollableControl1.Visible = was_visible;
            _table.ResumeLayout(true);
            scrollableControl1.ResumeLayout(true);
            this.ResumeLayout(true);

            _init = true;
        }


        private void checkbox_checked_changed(object sender, EventArgs e)
        {
            if (!_init) return;
            if (CheckboxChanged != null) CheckboxChanged(this, EventArgs.Empty);
        }

        public int GetBitfield(int bit_group)
        {
            int bitfield_value = 0;
            List<(int bit, CheckBox cb)> list;
            if (!_by_group.TryGetValue(bit_group, out list)) return 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].cb.Checked) bitfield_value |= (1 << list[i].bit);
            }
            return bitfield_value;
        }

        public void SetBitfield(int bit_group, int value)
        {
            bool old_init = _init;
            _init = false;
            List<(int bit, CheckBox cb)> list;
            if (_by_group.TryGetValue(bit_group, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    bool is_checked = (value & (1 << list[i].bit)) != 0;
                    list[i].cb.Checked = is_checked;
                }
            }
            _init = old_init;
        }

        public int GetCheckedCount(int bit_group)
        {
            int count = 0;
            List<(int bit, CheckBox cb)> list;
            if (!_by_group.TryGetValue(bit_group, out list)) return 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].cb.Checked) count++;
            }
            return count;
        }
    }
}

