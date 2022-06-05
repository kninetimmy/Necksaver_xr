using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace XRNeckSafer
{

    public partial class MainForm : Form
    {
        public JoystickStuff js;
        public VRStuff vr;
        public Config conf;
        public Graph gr;



        public int joy_offset_angle;
        public int auto_offset_angle;
        public int sum_offset_angle;
        public int last_offset_angle;
        public float last_offset_x;
        public float last_offset_z;

        public float trans_offset_LR;
        public float trans_offset_F;
        public Vector3 trans_offset;
        public Vector3 auto_trans_offset;

        public int hmdYaw;

        public bool lastpressed;

        public bool autorot_config_error;

        public int min_form_heigh;

        public MainForm()
        {

            conf = Config.ReadConfig();

            InitializeComponent();
            min_form_heigh = Height;
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            this.showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            this.exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;

            if (conf.StartMinimized) this.WindowState = FormWindowState.Minimized;

            js = new JoystickStuff();
            vr = new VRStuff();

            angleNUD.Value = conf.Angle;
            transFNUP.Value = conf.TransF;
            transLRNUP.Value = conf.TransLR;
            additivRB.Checked = conf.Additiv;
            autoCB.Checked = conf.Auto;
            if (conf.Auto) enableAuto(true);
            else enableAuto(false);

            setMenuCheckmarks();

            for (int i = 0; i < conf.AutoSteps.Count; i++)
            {
                string[] r = new string[5]
                {
                    conf.AutoSteps[i][0].ToString(),
                    conf.AutoSteps[i][1].ToString(),
                    conf.AutoSteps[i][2].ToString(),
                    conf.AutoSteps[i][3].ToString(),
                    conf.AutoSteps[i][4].ToString(),
                };
                AutorotGridView.Rows.Add(r);
            }
            AutorotGridView.EnableHeadersVisualStyles = false;
            AutorotGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            AutorotGridView.RowHeadersVisible = false;
            AutorotGridView.Columns[0].HeaderText = @"act";
            AutorotGridView.Columns[0].HeaderCell.Style.Font = DefaultFont;
            AutorotGridView.Columns[0].HeaderCell.Style.ForeColor = System.Drawing.Color.Red;
            AutorotGridView.Columns[0].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            AutorotGridView.Columns[1].HeaderText = @"de";
            AutorotGridView.Columns[1].HeaderCell.Style.Font = DefaultFont;
            AutorotGridView.Columns[1].HeaderCell.Style.ForeColor = System.Drawing.Color.Green;
            AutorotGridView.Columns[1].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            AutorotGridView.Columns[2].HeaderText = @"rot";
            AutorotGridView.Columns[2].HeaderCell.Style.Font = DefaultFont;
            AutorotGridView.Columns[2].HeaderCell.Style.ForeColor = System.Drawing.Color.Black;
            AutorotGridView.Columns[2].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            AutorotGridView.Columns[3].HeaderText = @"L/R";
            AutorotGridView.Columns[3].HeaderCell.Style.Font = DefaultFont;
            AutorotGridView.Columns[3].HeaderCell.Style.ForeColor = System.Drawing.Color.Blue;
            AutorotGridView.Columns[3].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            AutorotGridView.Columns[4].HeaderText = @"Fwd";
            AutorotGridView.Columns[4].HeaderCell.Style.Font = DefaultFont;
            AutorotGridView.Columns[4].HeaderCell.Style.ForeColor = System.Drawing.Color.CadetBlue;
            AutorotGridView.Columns[4].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;


            setButtonToolTip(SetLeftButton, conf.LeftButton);
            setButtonToolTip(SetRightButton, conf.RightButton);
            setButtonToolTip(SetResetButton, conf.ResetButton);
            setButtonToolTip(SetHoldButton1, conf.HoldButton1);
            setButtonToolTip(SetHoldButton2, conf.HoldButton2);
            setButtonToolTip(SetHoldButton3, conf.HoldButton3);
            setButtonToolTip(SetHoldButton4, conf.HoldButton4);

            setLabelToolTip(LeftLabel, conf.LeftButton);
            setLabelToolTip(RightLabel, conf.RightButton);

            error_label.Visible = check_autorot_config();
            error_label2.Visible = error_label.Visible;
            loopTimer.Start();
        }

        private void enableAuto(bool enable)
        {
            AddButton.Enabled = enable;
            DeleteButton.Enabled = enable;
            graphButton.Enabled = enable;
            SetHoldButton1.Enabled = enable;
            SetHoldButton2.Enabled = enable;
            SetHoldButton3.Enabled = enable;
            SetHoldButton4.Enabled = enable;
            label2.Enabled = enable;
            AutorotGridView.Enabled = enable;
            AutorotGridView.ForeColor = enable ? SystemColors.ControlText : System.Drawing.Color.Gray;
            if (!enable) auto_offset_angle = 0;
        }

        public void setButtonToolTip(Button b, ButtonConfig bc)
        {
            string Text = js.NameFromGuid(bc.JoystickGUID) + ": " + bc.Button;
            if (bc.UseModifier)
            {
                Text += "   +   " + js.NameFromGuid(bc.ModJoystickGUID) + ": " + bc.ModButton;
            }
            toolTip1.SetToolTip(b, Text);
        }

        private void setLabelToolTip(Label l, ButtonConfig bc)
        {
            string Text = js.NameFromGuid(bc.JoystickGUID) + ": " + bc.Button;
            if (bc.UseModifier)
            {
                Text += "   +   " + js.NameFromGuid(bc.ModJoystickGUID) + ": " + bc.ModButton;
            }
            toolTip1.SetToolTip(l, Text);
        }

        private void angleNUD_ValueChanged(object sender, EventArgs e)
        {
            conf.Angle = (int)angleNUD.Value;
            conf.WriteConfig();
        }
        private void angleNUD_KeyUp(object sender, KeyEventArgs e)
        {
            conf.Angle = (int)angleNUD.Value;
            conf.WriteConfig();
        }

        private void additivRB_CheckedChanged(object sender, EventArgs e)
        {
            conf.Additiv = additivRB.Checked;
            transLRNUP.Enabled = !additivRB.Checked;
            transFNUP.Enabled = !additivRB.Checked;
            label14.Enabled = !additivRB.Checked;
            label15.Enabled = !additivRB.Checked;
            label16.Enabled = !additivRB.Checked;
            label17.Enabled = !additivRB.Checked;
            conf.WriteConfig();
        }

        private void autoCB_CheckedChanged(object sender, EventArgs e)
        {
            conf.Auto = autoCB.Checked;
            enableAuto(autoCB.Checked);
            conf.WriteConfig();
        }

        bool checkButtonPress(Button b, ButtonConfig bc)
        {
            bool pressed = js.IsButtonPressed(bc);
            if (pressed)
            {
                b.ForeColor = System.Drawing.Color.LightGreen;
                b.BackColor = SystemColors.ControlText;
            }
            else
            {
                b.ForeColor = SystemColors.ControlText;
                b.BackColor = SystemColors.ButtonFace;
            }
            return pressed;
        }

        private void loopTimer_Tick(object sender, EventArgs e)
        {
            bool reset_pressed = checkButtonPress(SetResetButton, conf.ResetButton);
            bool acc_res_pressed = js.IsButtonPressed(conf.AccuResetButton);
            bool l_pressed = js.IsButtonPressed(conf.LeftButton);
            bool r_pressed = js.IsButtonPressed(conf.RightButton);
            if (conf.MultipleLRbuttons)
            {
                l_pressed |= js.IsButtonPressed(conf.LeftButton2);
                l_pressed |= js.IsButtonPressed(conf.LeftButton3);
                r_pressed |= js.IsButtonPressed(conf.RightButton2);
                r_pressed |= js.IsButtonPressed(conf.RightButton3);
                reset_pressed |= js.IsButtonPressed(conf.ResetButton2);
                reset_pressed |= js.IsButtonPressed(conf.ResetButton3);
                acc_res_pressed |= js.IsButtonPressed(conf.AccuResetButton2);
                acc_res_pressed |= js.IsButtonPressed(conf.AccuResetButton3);
            }
            bool h1 = checkButtonPress(SetHoldButton1, conf.HoldButton1);
            bool h2 = checkButtonPress(SetHoldButton2, conf.HoldButton2);
            bool h3 = checkButtonPress(SetHoldButton3, conf.HoldButton3);
            bool h4 = checkButtonPress(SetHoldButton4, conf.HoldButton4);
            bool pitchlimit = vr.getHmdPitch() - 90 > conf.PitchLimForAutorot;

            bool autofrozen = h1 || h2 || h3 || h4 || pitchlimit;

            if (l_pressed)
            {
                LeftLabel.ForeColor = System.Drawing.Color.LightGreen;
                LeftLabel.BackColor = SystemColors.ControlText;
                SetLeftButton.ForeColor = System.Drawing.Color.LightGreen;
                SetLeftButton.BackColor = SystemColors.ControlText;
            }
            else
            {
                LeftLabel.ForeColor = SystemColors.ControlText;
                LeftLabel.BackColor = SystemColors.Control;
                SetLeftButton.ForeColor = SystemColors.ControlText;
                SetLeftButton.BackColor = SystemColors.Control;
            }
            if (r_pressed)
            {
                RightLabel.ForeColor = System.Drawing.Color.LightGreen;
                RightLabel.BackColor = SystemColors.ControlText;
                SetRightButton.ForeColor = System.Drawing.Color.LightGreen;
                SetRightButton.BackColor = SystemColors.ControlText;
            }
            else
            {
                RightLabel.ForeColor = SystemColors.ControlText;
                RightLabel.BackColor = SystemColors.Control;
                SetRightButton.ForeColor = SystemColors.ControlText;
                SetRightButton.BackColor = SystemColors.Control;
            }
            if (reset_pressed)
            {
                SetResetButton.ForeColor = System.Drawing.Color.LightGreen;
                SetResetButton.BackColor = SystemColors.ControlText;
            }
            else
            {
                SetResetButton.ForeColor = SystemColors.ControlText;
                SetResetButton.BackColor = SystemColors.Control;
            }
            if (acc_res_pressed)
            {
                AccumReset.ForeColor = System.Drawing.Color.LightGreen;
                AccumReset.BackColor = SystemColors.ControlText;
            }
            else
            {
                AccumReset.ForeColor = SystemColors.ControlText;
                AccumReset.BackColor = SystemColors.Control;
            }

            trans_offset = new Vector3(0, 0, 0);

            vr.updateHmdOrientation();

            float hmdYaw = vr.getHmdYaw();

            while (hmdYaw < -180) hmdYaw += 360;
            while (hmdYaw > 180) hmdYaw -= 360;

            //            if (vr.HmdIsActive())
            HMDYawLabel.Text = "HMD yaw: " + Math.Round(hmdYaw) + " deg";
            //            else
            //                HMDYawLabel.Text = "HMD yaw: standby";

            if (reset_pressed)
            {
                vr.resetHmdOrientation();
                joy_offset_angle = 0;
            }

            if (additivRB.Checked)
            {
                if (l_pressed && !lastpressed)
                    joy_offset_angle -= (int)angleNUD.Value;
                if (r_pressed && !lastpressed)
                    joy_offset_angle += (int)angleNUD.Value;
                if (acc_res_pressed)
                    joy_offset_angle = 0;
            }
            else
            {
                if (l_pressed)
                {
                    joy_offset_angle = -(int)angleNUD.Value;
                    trans_offset.X = trans_offset_LR;
                    trans_offset.Z = trans_offset_F;
                }
                else if (r_pressed)
                {
                    joy_offset_angle = (int)angleNUD.Value;
                    trans_offset.X = -trans_offset_LR;
                    trans_offset.Z = trans_offset_F;
                }
                else
                {
                    joy_offset_angle = 0;
                    trans_offset.X = 0;
                    trans_offset.Z = 0;
                }
            }

            if (autoCB.Checked)
            {
                if (autofrozen)
                {
                    AutorotLabel.Text = "Autorotation - on hold";
                    if (pitchlimit) AutorotLabel.Text += " (pitch limit)";
                    else AutorotLabel.Text += " (button)";
                }
                else
                {
                    AutorotLabel.Text = "Autorotation";
                    calcAutoRotAndTrans((int)hmdYaw, ref auto_offset_angle, ref auto_trans_offset);
                }
            }


            sum_offset_angle = joy_offset_angle + auto_offset_angle;
            if (Math.Abs(auto_trans_offset.X) > Math.Abs(trans_offset.X)) trans_offset.X = auto_trans_offset.X;
            if (Math.Abs(auto_trans_offset.Z) > Math.Abs(trans_offset.Z)) trans_offset.Z = auto_trans_offset.Z;


            if (last_offset_angle != sum_offset_angle
                || last_offset_x != trans_offset.X
                || last_offset_z != trans_offset.Z)
            {
                vr.setOffset(sum_offset_angle, trans_offset);
            }

            lastpressed = l_pressed || r_pressed;

            last_offset_angle = sum_offset_angle;
            last_offset_x = trans_offset.X;
            last_offset_z = trans_offset.Z;

            Text = "XRNS (" + sum_offset_angle + " deg)";

            if (gr != null)
            {
                if (gr.hmd != hmdYaw)
                {
                    gr.hmd = (int)hmdYaw;
                    gr.rot = (int)hmdYaw + sum_offset_angle;
                    gr.Refresh();
                }
            }

        }

        private void calcAutoRotAndTrans(int yaw, ref int arot, ref Vector3 atrans)
        {
            int yawsign = (yaw > 0) ? 1 : -1;
            int absyaw = yaw * yawsign;
            int arotsign = (arot > 0) ? 1 : -1;
            int absarot = arot * arotsign;
            int autorot = 0;
            int transx = 0;
            int transz = 0;


            int act;
            int deact = 0;
            int rot;
            int tx;
            int tz;

            for (int i = 0; i < conf.AutoSteps.Count; i++)
            {
                act = conf.AutoSteps[i][0];
                deact = conf.AutoSteps[i][1];
                rot = conf.AutoSteps[i][2];
                tx = conf.AutoSteps[i][3];
                tz = conf.AutoSteps[i][4];

                if (absyaw >= act)
                {
                    autorot = rot;
                    transx = tx;
                    transz = tz;
                }
                else
                {
                    break;
                }
            }

            if ((absarot > autorot) && (absyaw >= deact))
            {
                return;
            }
            arot = yawsign * autorot;
            atrans.X = (float)transx / 100.0F * -yawsign;
            atrans.Z = (float)transz / 100.0F;
        }

        private void transFNUP_ValueChanged(object sender, EventArgs e)
        {
            conf.TransF = (int)transFNUP.Value;
            trans_offset_F = (float)transFNUP.Value / 100F;
            conf.WriteConfig();
        }
        private void transFNUP_KeyUp(object sender, KeyEventArgs e)
        {
            conf.TransF = (int)transFNUP.Value;
            trans_offset_F = (float)transFNUP.Value / 100F;
            conf.WriteConfig();
        }

        private void transLRNUP_ValueChanged(object sender, EventArgs e)
        {
            conf.TransLR = (int)transLRNUP.Value;
            trans_offset_LR = (float)transLRNUP.Value / 100F;
            conf.WriteConfig();
        }

        private void transLRNUP_KeyUp(object sender, KeyEventArgs e)
        {
            conf.TransLR = (int)transLRNUP.Value;
            trans_offset_LR = (float)transLRNUP.Value / 100F;
            conf.WriteConfig();
        }



        private void AddButton_Click(object sender, EventArgs e)
        {
            int[] i = new int[5];
            i[0] = conf.AutoSteps[conf.AutoSteps.Count - 1][0] + 10;
            i[1] = conf.AutoSteps[conf.AutoSteps.Count - 1][0] + 1;
            i[2] = conf.AutoSteps[conf.AutoSteps.Count - 1][2] + 10;
            i[3] = conf.AutoSteps[conf.AutoSteps.Count - 1][3];
            i[4] = conf.AutoSteps[conf.AutoSteps.Count - 1][4];
            conf.AutoSteps.Add(i);
            string[] s = new string[5]
            {
                conf.AutoSteps[conf.AutoSteps.Count-1][0].ToString(),
                conf.AutoSteps[conf.AutoSteps.Count-1][1].ToString(),
                conf.AutoSteps[conf.AutoSteps.Count-1][2].ToString(),
                conf.AutoSteps[conf.AutoSteps.Count-1][3].ToString(),
                conf.AutoSteps[conf.AutoSteps.Count-1][4].ToString(),
            };
            AutorotGridView.Rows.Add(s);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (conf.AutoSteps.Count > 1)
            {
                conf.AutoSteps.RemoveAt(conf.AutoSteps.Count - 1);
                AutorotGridView.Rows.Remove(AutorotGridView.Rows[AutorotGridView.RowCount - 1]);
            }
        }

        private bool check_autorot_config()
        {
            int val;

            bool error = false;

            for (int col = 0; col < AutorotGridView.ColumnCount; col++)
            {
                for (int row = 0; row < AutorotGridView.RowCount; row++)
                {
                    string s = AutorotGridView[col, row].Value.ToString();
                    bool good = int.TryParse(s, out val);

                    if (good)
                    {
                        if (val < 0) good = false;
                        if (row < AutorotGridView.RowCount - 1 && col == 0)
                        {
                            if (val >= conf.AutoSteps[row + 1][1]) good = false;
                            if (val >= conf.AutoSteps[row + 1][0]) good = false;
                        }

                        if (row > 0 && col == 0 && val <= conf.AutoSteps[row - 1][0]) good = false;
                        if (row > 0 && col == 1 && val <= conf.AutoSteps[row - 1][0]) good = false;
                        if (col == 0 && val <= conf.AutoSteps[row][1]) good = false;
                        if (col == 1 && val >= conf.AutoSteps[row][0]) good = false;
                        if (col == 3 && val > 40) good = false;
                        if (col == 4 && val > 20) good = false;
                    }

                    if (good)
                    {
                        AutorotGridView.Rows[row].Cells[col].Style.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        AutorotGridView.Rows[row].Cells[col].Style.BackColor = System.Drawing.Color.Red;
                        error = true;
                    }
                }
            }
            return error;
        }

        private void AutorotGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int val;

            if (e.RowIndex == -1) return;

            string s = AutorotGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
            bool good = int.TryParse(s, out val);

            if (good)
            {
                conf.AutoSteps[e.RowIndex][e.ColumnIndex] = val;
                conf.WriteConfig();
            }

            error_label.Visible = check_autorot_config();
            error_label2.Visible = error_label.Visible;
            if (gr != null)
                gr.Graph_ValuesChanged();
        }

        private void AutorotGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
            AutorotGridView.MaximumSize = new System.Drawing.Size(AutorotGridView.Width, Size.Height - groupAuto.Location.Y - 111);
            MaximumSize = new System.Drawing.Size(MaximumSize.Width, Math.Max(min_form_heigh, AutorotGridView.RowCount * 22 + 406));
            conf.WriteConfig();
            if (gr != null)
                gr.Graph_ValuesChanged();
        }

        private void AutorotGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
            AutorotGridView.MaximumSize = new System.Drawing.Size(AutorotGridView.Width, Size.Height - groupAuto.Location.Y - 111);
            MaximumSize = new System.Drawing.Size(MaximumSize.Width, Math.Max(min_form_heigh, AutorotGridView.RowCount * 22 + 406));
            conf.WriteConfig();
            if (gr != null)
                gr.Graph_ValuesChanged();
        }

        private void startMinimzedToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            conf.StartMinimized = startMinimzedToolStripMenuItem.Checked;
            conf.WriteConfig();
        }

        private void minimizeToTrayToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            conf.MinimizeToTray = minimizeToTrayToolStripMenuItem.Checked;
            conf.WriteConfig();
        }

        private void SetLeftButton_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Button for Left Rotation:", conf.LeftButton);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Left", conf.LeftButton, conf.LeftButton2, conf.LeftButton3);
                frm.ShowDialog();
            }
            setButtonToolTip(SetLeftButton, conf.LeftButton);
            setLabelToolTip(LeftLabel, conf.LeftButton);
        }

        private void SetRightButton_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Button for Right Rotation:", conf.RightButton);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Right", conf.RightButton, conf.RightButton2, conf.RightButton3);
                frm.ShowDialog();
            }
            setButtonToolTip(SetRightButton, conf.RightButton);
            setLabelToolTip(RightLabel, conf.RightButton);
        }

        private void SetResetButton_Click(object sender, EventArgs e)
        {

            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Reset Button:", conf.ResetButton);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Reset", conf.ResetButton, conf.ResetButton2, conf.ResetButton3);
                frm.ShowDialog();
            }
        }
        private void AccumReset_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Accum Reset Button:", conf.AccuResetButton);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Accum Reset", conf.AccuResetButton, conf.AccuResetButton2, conf.AccuResetButton3);
                frm.ShowDialog();
            }
        }

        private void SetHoldButton1_Click(object sender, EventArgs e)
        {
            ButtonForm frm = new ButtonForm(this, "Button for Reset:", conf.HoldButton1);
            frm.ShowDialog();
            setButtonToolTip(SetHoldButton1, conf.HoldButton1);
        }

        private void SetHoldButton2_Click(object sender, EventArgs e)
        {
            ButtonForm frm = new ButtonForm(this, "Button for Reset:", conf.HoldButton2);
            frm.ShowDialog();
            setButtonToolTip(SetHoldButton2, conf.HoldButton2);
        }

        private void SetHoldButton3_Click(object sender, EventArgs e)
        {
            ButtonForm frm = new ButtonForm(this, "Button for Reset:", conf.HoldButton3);
            frm.ShowDialog();
            setButtonToolTip(SetHoldButton3, conf.HoldButton3);
        }

        private void SetHoldButton4_Click(object sender, EventArgs e)
        {
            ButtonForm frm = new ButtonForm(this, "Button for Reset:", conf.HoldButton4);
            frm.ShowDialog();
            setButtonToolTip(SetHoldButton4, conf.HoldButton4);
        }

        void sizeChanged()
        {
            VersionLabel.Location = new System.Drawing.Point(VersionLabel.Location.X, Size.Height - 56);
            groupAuto.Height = Size.Height - groupAuto.Location.Y - 59;

            AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
            AutorotGridView.MaximumSize = new System.Drawing.Size(AutorotGridView.Width, Size.Height - groupAuto.Location.Y - 111);
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            sizeChanged();
        }

        private void sendToTrayIfNeeded()
        {
            if (conf.MinimizeToTray)
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    Hide();
                    notifyIcon.Visible = true;
                }
                else
                {
                    Show();
                    notifyIcon.Visible = false;
                }
            }
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            sizeChanged();
            sendToTrayIfNeeded();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (conf.MinimizeToTray && conf.StartMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }

        private void setMenuCheckmarks()
        {
            if (conf.StartMinimized) startMinimzedToolStripMenuItem.Checked = true;
            if (conf.MinimizeToTray) minimizeToTrayToolStripMenuItem.Checked = true;
            if (conf.MultipleLRbuttons) MultipleLRButtonsToolStripMenuItem.Checked = true;

            ToolStripMenuItem item = (ToolStripMenuItem)PitchLimToolStripMenuItem.DropDownItems[conf.PitchLimForAutorot / 10 - 1];
            item.Checked = true;
        }

        private void resetOptionsToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conf.StartMinimized = false;
            conf.MinimizeToTray = false;
            conf.WriteConfig();
            setMenuCheckmarks();
        }

        private void graphButton_Click(object sender, EventArgs e)
        {
            gr = new Graph(this);
            gr.Show();
        }

        private void PitchLimToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem item in PitchLimToolStripMenuItem.DropDownItems) item.Checked = false;
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
            int.TryParse(e.ClickedItem.Text.Substring(0, 2), out conf.PitchLimForAutorot);
            conf.WriteConfig();
        }

        private void moreLRButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MultipleLRButtonsToolStripMenuItem.Checked)
            {
                MultipleLRButtonsToolStripMenuItem.Checked = false;
                conf.MultipleLRbuttons = false;
                conf.WriteConfig();
            }
            else
            {
                MultipleLRButtonsToolStripMenuItem.Checked = true;
                conf.MultipleLRbuttons = true;
                conf.WriteConfig();
            }
        }

        private void listApiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<String> LayerNames = vr.ListApiLayers();

            string message = "";
            string title = "OpenXR API Layers";
            foreach (string name in LayerNames)
            {
                message = message + "\n" + name;
            }
            MessageBox.Show(message, title);
        }
    }
}
