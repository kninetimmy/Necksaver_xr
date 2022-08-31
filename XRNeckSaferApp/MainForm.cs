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
        public int joy_offset_angle_pitch;
        public int auto_offset_angle_pitch;
        public int sum_offset_angle_pitch;
        public int last_offset_angle_pitch;
        public float last_offset_x;
        public float last_offset_z;

        public float trans_offset_LR;
        public float trans_offset_F;
        public Vector3 trans_offset;
        public Vector3 auto_trans_offset;

        public int hmdYaw;

        public bool lastpressed;
        public bool lastpitchpressed;
        public bool last_h_pressed;
        public bool last_hp_pressed;
        public string ARText;
        public string pARText;
        public string HMDtext;
        public bool autorot_config_error;


        public MainForm()
        {

            conf = Config.ReadConfig();

            InitializeComponent();

            VersionLabel.Text = "beta3c";
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            this.showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            this.exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;

            if (conf.StartMinimized) this.WindowState = FormWindowState.Minimized;

            js = new JoystickStuff();
            vr = new VRStuff();

            angleNUD.Value = conf.Angle;
            upNUD.Value = conf.UpAngle;
            downNUD.Value = conf.DownAngle;
            transFNUP.Value = conf.TransF;
            transLRNUP.Value = conf.TransLR;
            additivRB.Checked = conf.Additiv;
            if (conf.AutoMode == "stepwise")
            {
                ARstepwise.Checked = true;
            }
            else if (conf.AutoMode == "linear")
            {
                ARlinear.Checked = true;
            }
            else
            {
                AROffButton.Checked = true;
            }

            if (conf.PitchAutoMode == "stepwise")
            {
                pARstepwise.Checked = true;
            }
            else if (conf.PitchAutoMode == "linear")
            {
                pARlinear.Checked = true;
            }
            else
            {
                pAROffButton.Checked = true;
            }
            pitchAutorotChanged(new Object(), new EventArgs());
            autorot_changed(new Object(), new EventArgs());
            YawPitchTab.Height = ManualGroup.Height + ARGroup.Height + 50;
            Height = YawPitchTab.Location.Y + YawPitchTab.Height + 60;

            numericUpDownStartLeft.Value = conf.LinearLimL;
            numericUpDownStartRight.Value = conf.LinearLimR;
            numericUpDownMultLeft.Value = conf.LinearMultL;
            numericUpDownMultRight.Value = conf.LinearMultR;

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

            for (int i = 0; i < conf.UpAutoSteps.Count; i++)
            {
                string[] r = new string[3]
                {
                    conf.UpAutoSteps[i][0].ToString(),
                    conf.UpAutoSteps[i][1].ToString(),
                    conf.UpAutoSteps[i][2].ToString(),
                };
                UpAutorotGridView.Rows.Add(r);
            }
            UpAutorotGridView.EnableHeadersVisualStyles = false;
            UpAutorotGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            UpAutorotGridView.RowHeadersVisible = false;
            UpAutorotGridView.Columns[0].HeaderText = @"act";
            UpAutorotGridView.Columns[0].HeaderCell.Style.Font = DefaultFont;
            UpAutorotGridView.Columns[0].HeaderCell.Style.ForeColor = System.Drawing.Color.Red;
            UpAutorotGridView.Columns[0].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            UpAutorotGridView.Columns[1].HeaderText = @"de";
            UpAutorotGridView.Columns[1].HeaderCell.Style.Font = DefaultFont;
            UpAutorotGridView.Columns[1].HeaderCell.Style.ForeColor = System.Drawing.Color.Green;
            UpAutorotGridView.Columns[1].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            UpAutorotGridView.Columns[2].HeaderText = @"rot";
            UpAutorotGridView.Columns[2].HeaderCell.Style.Font = DefaultFont;
            UpAutorotGridView.Columns[2].HeaderCell.Style.ForeColor = System.Drawing.Color.Black;
            UpAutorotGridView.Columns[2].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;

            for (int i = 0; i < conf.DownAutoSteps.Count; i++)
            {
                string[] r = new string[3]
                {
                    conf.DownAutoSteps[i][0].ToString(),
                    conf.DownAutoSteps[i][1].ToString(),
                    conf.DownAutoSteps[i][2].ToString(),
                };
                DownAutorotGridView.Rows.Add(r);
            }
            DownAutorotGridView.EnableHeadersVisualStyles = false;
            DownAutorotGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            DownAutorotGridView.RowHeadersVisible = false;
            DownAutorotGridView.Columns[0].HeaderText = @"act";
            DownAutorotGridView.Columns[0].HeaderCell.Style.Font = DefaultFont;
            DownAutorotGridView.Columns[0].HeaderCell.Style.ForeColor = System.Drawing.Color.Red;
            DownAutorotGridView.Columns[0].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            DownAutorotGridView.Columns[1].HeaderText = @"de";
            DownAutorotGridView.Columns[1].HeaderCell.Style.Font = DefaultFont;
            DownAutorotGridView.Columns[1].HeaderCell.Style.ForeColor = System.Drawing.Color.Green;
            DownAutorotGridView.Columns[1].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;
            DownAutorotGridView.Columns[2].HeaderText = @"rot";
            DownAutorotGridView.Columns[2].HeaderCell.Style.Font = DefaultFont;
            DownAutorotGridView.Columns[2].HeaderCell.Style.ForeColor = System.Drawing.Color.Black;
            DownAutorotGridView.Columns[2].HeaderCell.Style.BackColor = System.Drawing.Color.LightGray;

            setButtonToolTip(SetLeftButton, conf.LeftButton);
            setButtonToolTip(SetRightButton, conf.RightButton);
            setButtonToolTip(SetResetButton, conf.ResetButton);
            setButtonToolTip(AccumReset, conf.AccuResetButton);
            setButtonToolTip(SetHoldButton1, conf.HoldButton1);

            setLabelToolTip(LeftLabel, conf.LeftButton);
            setLabelToolTip(RightLabel, conf.RightButton);

            error_label.Visible = check_autorot_config();
            error_label2.Visible = error_label.Visible;
            upErrorLabel1.Visible = check_autorot_config();
            upErrorLabel2.Visible = upErrorLabel1.Visible;
            downErrorLabel1.Visible = check_autorot_config();
            downErrorLabel2.Visible = downErrorLabel1.Visible;
            numericUpDownStartLeft.Value = conf.LinearLimL;
            numericUpDownStartRight.Value = conf.LinearLimR;
            numericUpDownMultLeft.Value = conf.LinearMultL;
            numericUpDownMultRight.Value = conf.LinearMultR;
            numericUpDownStartUp.Value = conf.LinearLimU;
            numericUpDownStartDown.Value = conf.LinearLimD;
            numericUpDownMultUp.Value = conf.LinearMultU;
            numericUpDownMultDown.Value = conf.LinearMultD;

            vr.setLinearRotationSettings(conf.AutoMode == "linear", conf.LinearLimL, conf.LinearLimR, conf.LinearMultL, conf.LinearMultR);
            vr.setPitchLinearRotationSettings(conf.PitchAutoMode == "linear", conf.LinearLimU, conf.LinearLimD, conf.LinearMultU, conf.LinearMultD);
            ARText = "Autorotation";
            pARText = "Autorotation";
            HMDtext = "";
            loopTimer.Start();
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
        private void upNUD_ValueChanged(object sender, EventArgs e)
        {
            conf.UpAngle = (int)upNUD.Value;
            conf.WriteConfig();
        }
        private void upNUD_KeyUp(object sender, KeyEventArgs e)
        {
            conf.Angle = (int)upNUD.Value;
            conf.WriteConfig();
        }
        private void downNUD_ValueChanged(object sender, EventArgs e)
        {
            conf.DownAngle = (int)downNUD.Value;
            conf.WriteConfig();
        }
        private void downNUD_KeyUp(object sender, KeyEventArgs e)
        {
            conf.DownAngle = (int)downNUD.Value;
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

        private void setButtonColor(bool pressed, Button b)
        {
            System.Drawing.Color fc = SystemColors.ControlText;
            System.Drawing.Color bc = SystemColors.Control;

            if (pressed)
            {
                fc = System.Drawing.Color.LightGreen;
                bc = SystemColors.ControlText;
            }

            if (b.ForeColor != fc)
            {
                b.ForeColor = fc;
                b.BackColor = bc;
            }
        }
        void setLabelColor(bool pressed, Label l)
        {
            System.Drawing.Color fc = SystemColors.ControlText;
            System.Drawing.Color bc = SystemColors.Control;

            if (pressed)
            {
                fc = System.Drawing.Color.LightGreen;
                bc = SystemColors.ControlText;
            }

            if (l.ForeColor != fc)
            {
                l.ForeColor = fc;
                l.BackColor = bc;
            }
        }

        private void loopTimer_Tick(object sender, EventArgs e)
        {
            bool reset_pressed = js.IsButtonPressed(conf.ResetButton);
            bool acc_res_pressed = js.IsButtonPressed(conf.AccuResetButton);
            bool pitch_acc_res_pressed = js.IsButtonPressed(conf.PitchAccuResetButton);
            bool l_pressed = js.IsButtonPressed(conf.LeftButton);
            bool r_pressed = js.IsButtonPressed(conf.RightButton);
            bool u_pressed = js.IsButtonPressed(conf.UpButton);
            bool d_pressed = js.IsButtonPressed(conf.DownButton);
            bool h_pressed = js.IsButtonPressed(conf.HoldButton1);
            bool hp_pressed = js.IsButtonPressed(conf.PitchHoldButton1);
            //            bool h_pressed = checkButtonPress(SetHoldButton1, conf.HoldButton1);
            if (conf.MultipleLRbuttons)
            {
                l_pressed |= js.IsButtonPressed(conf.LeftButton2);
                l_pressed |= js.IsButtonPressed(conf.LeftButton3);
                r_pressed |= js.IsButtonPressed(conf.RightButton2);
                r_pressed |= js.IsButtonPressed(conf.RightButton3);
                u_pressed |= js.IsButtonPressed(conf.UpButton2);
                u_pressed |= js.IsButtonPressed(conf.UpButton3);
                d_pressed |= js.IsButtonPressed(conf.DownButton2);
                d_pressed |= js.IsButtonPressed(conf.DownButton3);
                reset_pressed |= js.IsButtonPressed(conf.ResetButton2);
                reset_pressed |= js.IsButtonPressed(conf.ResetButton3);
                acc_res_pressed |= js.IsButtonPressed(conf.AccuResetButton2);
                acc_res_pressed |= js.IsButtonPressed(conf.AccuResetButton3);
                pitch_acc_res_pressed |= js.IsButtonPressed(conf.PitchAccuResetButton2);
                pitch_acc_res_pressed |= js.IsButtonPressed(conf.PitchAccuResetButton3);
                h_pressed |= js.IsButtonPressed(conf.HoldButton2);
                h_pressed |= js.IsButtonPressed(conf.HoldButton3);
                hp_pressed |= js.IsButtonPressed(conf.PitchHoldButton2);
                hp_pressed |= js.IsButtonPressed(conf.PitchHoldButton3);
            }

            setButtonColor(l_pressed, SetLeftButton);
            setLabelColor(l_pressed, LeftLabel);
            setButtonColor(r_pressed, SetRightButton);
            setLabelColor(r_pressed, RightLabel);
            setButtonColor(u_pressed, SetUpButton);
            setLabelColor(u_pressed, UpLabel);
            setButtonColor(d_pressed, SetDownButton);
            setLabelColor(d_pressed, DownLabel);
            setButtonColor(reset_pressed, SetResetButton);
            setButtonColor(acc_res_pressed, AccumReset);
            setButtonColor(h_pressed, SetHoldButton1);
            setButtonColor(hp_pressed, SetPitchHoldButton);

            bool pitchlimit = vr.getHmdPitch() - 90 > conf.PitchLimForAutorot;

            bool autofrozen = h_pressed || pitchlimit;

            if (h_pressed != last_h_pressed)
            {
                vr.setLinearHold(h_pressed);
            }
            last_h_pressed = h_pressed;

            if (hp_pressed != last_hp_pressed)
            {
                vr.setPitchLinearHold(hp_pressed);
            }
            last_hp_pressed = hp_pressed;

            trans_offset = new Vector3(0, 0, 0);

            vr.updateHmdOrientation();

            float hmdYaw = vr.getHmdYaw();
            float hmdPitch = -vr.getHmdPitch();


            while (hmdYaw < -180) hmdYaw += 360;
            while (hmdYaw > 180) hmdYaw -= 360;

            if (vr.HmdWasCentered())
            {
                if (!conf.DisableGUIOutput)
                {
                    HMDtext = "HMD yaw: " + Math.Round(hmdYaw) + " deg   pitch: " + Math.Round(hmdPitch) + " deg";
                }
                else
                {
                    HMDtext = "     (HMD angle output disabled)";
                }
            }
            else
            {
                HMDtext = "HMD yaw: (not centered in game yet)";
            }

            if ((HMDYawLabel.Text != HMDtext))
            {
                HMDYawLabel.Location = new System.Drawing.Point(20, 18);
                HMDYawLabel.Text = HMDtext;
            }

            if (reset_pressed)
            {
                vr.resetHmdOrientation();
                joy_offset_angle = 0;
                vr.setLinearRotationSettings(conf.AutoMode == "linear", conf.LinearLimL, conf.LinearLimR, conf.LinearMultL, conf.LinearMultR);
                vr.setPitchLinearRotationSettings(conf.PitchAutoMode == "linear", conf.LinearLimU, conf.LinearLimD, conf.LinearMultU, conf.LinearMultD);
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

            if (pAdditivRB.Checked)
            {
                if (u_pressed && !lastpitchpressed)
                    joy_offset_angle_pitch += (int)upNUD.Value;
                if (d_pressed && !lastpitchpressed)
                    joy_offset_angle_pitch += (int)downNUD.Value;
                if (acc_res_pressed)
                    joy_offset_angle_pitch = 0;
            }
            else
            {
                if (u_pressed)
                {
                    joy_offset_angle_pitch = (int)upNUD.Value;
                }
                else if (d_pressed)
                {
                    joy_offset_angle_pitch = (int)-downNUD.Value;
                }
                else
                {
                    joy_offset_angle_pitch = 0;
                }
            }

            if (!AROffButton.Checked)
            {
                if (autofrozen)
                {
                    ARText = "Autorotation hold";
                    if (pitchlimit) ARText += " (pitch lim)";
                    else ARText += " (button)";
                }
                else
                {
                    ARText = "Autorotation";
                    if (conf.AutoMode == "stepwise")
                    {
                        calcAutoRotAndTrans((int)hmdYaw, ref auto_offset_angle, ref auto_trans_offset);
                    }
                    else
                    {

                        auto_offset_angle = 0;
                        auto_trans_offset.X = 0;
                        auto_trans_offset.Y = 0;
                        auto_trans_offset.Z = 0;
                    }
                }
            }

            if (ARGroup.Text != ARText)
            {
                ARGroup.Text = ARText;
            }

            if (!pAROffButton.Checked)
            {
                if (hp_pressed)
                {
                    pARText = "Autorotation hold (button)";
                }
                else
                {
                    pARText = "Autorotation";
                    if (conf.PitchAutoMode == "stepwise")
                    {
                        calcAutoPitch((int)hmdPitch, ref auto_offset_angle_pitch);
                    }
                    else
                    {
                        auto_offset_angle_pitch = 0;
                    }
                }
            }

            if (pARGroup.Text != pARText)
            {
                pARGroup.Text = pARText;
            }

            sum_offset_angle = joy_offset_angle + auto_offset_angle;
            sum_offset_angle_pitch = joy_offset_angle_pitch + auto_offset_angle_pitch;

            if (Math.Abs(auto_trans_offset.X) > Math.Abs(trans_offset.X)) trans_offset.X = auto_trans_offset.X;
            if (Math.Abs(auto_trans_offset.Z) > Math.Abs(trans_offset.Z)) trans_offset.Z = auto_trans_offset.Z;

            if (last_offset_angle != sum_offset_angle
                || last_offset_x != trans_offset.X
                || last_offset_z != trans_offset.Z
                || last_offset_angle_pitch != sum_offset_angle_pitch)
            {
                vr.setOffset(sum_offset_angle, sum_offset_angle_pitch, trans_offset);
                if (!conf.DisableGUIOutput)
                {
                    Text = "XRNS (Y:" + sum_offset_angle + "  P: " + sum_offset_angle_pitch + ")";
                }
            }

            lastpressed = l_pressed || r_pressed;
            lastpitchpressed = u_pressed || d_pressed;

            last_offset_angle = sum_offset_angle;
            last_offset_angle_pitch = sum_offset_angle_pitch;
            last_offset_x = trans_offset.X;
            last_offset_z = trans_offset.Z;

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
            int absarot = (arot > 0) ? arot : -arot;
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
        private void calcAutoPitch(int pitch, ref int arot)
        {
            List<int[]> Steps;
            int pitchsign = (pitch > 0) ? 1 : -1;
            int abspitch = (pitch > 0) ? pitch : -pitch;
            int autorot = 0;
            int absarot = (arot > 0) ? arot : -arot;

            int act;
            int deact = 0;
            int rot;

            Steps = (pitch > 0) ? conf.UpAutoSteps : conf.DownAutoSteps;

            for (int i = 0; i < Steps.Count; i++)
            {
                act = Steps[i][0];
                deact = Steps[i][1];
                rot = Steps[i][2];

                if (abspitch >= act)
                {
                    autorot = rot;
                }
                else
                {
                    break;
                }
            }

            if ((absarot > autorot) && (abspitch >= deact))
            {
                return;
            }

            arot = autorot * pitchsign;
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
        private void UpAddButton_Click(object sender, EventArgs e)
        {
            int[] i = new int[5];
            i[0] = conf.UpAutoSteps[conf.UpAutoSteps.Count - 1][0] + 10;
            i[1] = conf.UpAutoSteps[conf.UpAutoSteps.Count - 1][0] + 1;
            i[2] = conf.UpAutoSteps[conf.UpAutoSteps.Count - 1][2] + 10;
            conf.UpAutoSteps.Add(i);
            string[] s = new string[3]
            {
                conf.UpAutoSteps[conf.UpAutoSteps.Count-1][0].ToString(),
                conf.UpAutoSteps[conf.UpAutoSteps.Count-1][1].ToString(),
                conf.UpAutoSteps[conf.UpAutoSteps.Count-1][2].ToString(),
            };
            UpAutorotGridView.Rows.Add(s);
        }

        private void UpDeleteButton_Click(object sender, EventArgs e)
        {
            if (conf.UpAutoSteps.Count > 1)
            {
                conf.UpAutoSteps.RemoveAt(conf.UpAutoSteps.Count - 1);
                UpAutorotGridView.Rows.Remove(UpAutorotGridView.Rows[UpAutorotGridView.RowCount - 1]);
            }
        }
        private void DownAddButton_Click(object sender, EventArgs e)
        {
            int[] i = new int[5];
            i[0] = conf.DownAutoSteps[conf.DownAutoSteps.Count - 1][0] - 10;
            i[1] = conf.DownAutoSteps[conf.DownAutoSteps.Count - 1][0] - 1;
            i[2] = conf.DownAutoSteps[conf.DownAutoSteps.Count - 1][2] - 10;
            conf.DownAutoSteps.Add(i);
            string[] s = new string[3]
            {
                conf.DownAutoSteps[conf.DownAutoSteps.Count-1][0].ToString(),
                conf.DownAutoSteps[conf.DownAutoSteps.Count-1][1].ToString(),
                conf.DownAutoSteps[conf.DownAutoSteps.Count-1][2].ToString(),
            };
            DownAutorotGridView.Rows.Add(s);
        }

        private void DownDeleteButton_Click(object sender, EventArgs e)
        {
            if (conf.DownAutoSteps.Count > 1)
            {
                conf.DownAutoSteps.RemoveAt(conf.DownAutoSteps.Count - 1);
                DownAutorotGridView.Rows.Remove(DownAutorotGridView.Rows[DownAutorotGridView.RowCount - 1]);
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

        private bool check_UP_autorot_config()
        {
            int val;

            bool error = false;

            for (int col = 0; col < UpAutorotGridView.ColumnCount; col++)
            {
                for (int row = 0; row < UpAutorotGridView.RowCount; row++)
                {
                    string s = UpAutorotGridView[col, row].Value.ToString();
                    bool good = int.TryParse(s, out val);

                    if (good)
                    {
                        if (val < 0) good = false;
                        if (row < UpAutorotGridView.RowCount - 1 && col == 0)
                        {
                            if (val >= conf.UpAutoSteps[row + 1][1]) good = false;
                            if (val >= conf.UpAutoSteps[row + 1][0]) good = false;
                        }

                        if (row > 0 && col == 0 && val <= conf.UpAutoSteps[row - 1][0]) good = false;
                        if (row > 0 && col == 1 && val <= conf.UpAutoSteps[row - 1][0]) good = false;
                        if (col == 0 && val <= conf.UpAutoSteps[row][1]) good = false;
                        if (col == 1 && val >= conf.UpAutoSteps[row][0]) good = false;
                    }

                    if (good)
                    {
                        UpAutorotGridView.Rows[row].Cells[col].Style.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        UpAutorotGridView.Rows[row].Cells[col].Style.BackColor = System.Drawing.Color.Red;
                        error = true;
                    }
                }
            }
            return error;
        }
        private bool check_DOWN_autorot_config()
        {
            int val;

            bool error = false;

            for (int col = 0; col < DownAutorotGridView.ColumnCount; col++)
            {
                for (int row = 0; row < DownAutorotGridView.RowCount; row++)
                {
                    string s = DownAutorotGridView[col, row].Value.ToString();
                    bool good = int.TryParse(s, out val);

                    if (good)
                    {
                        if (val < 0) good = false;
                        if (row < DownAutorotGridView.RowCount - 1 && col == 0)
                        {
                            if (val >= conf.DownAutoSteps[row + 1][1]) good = false;
                            if (val >= conf.DownAutoSteps[row + 1][0]) good = false;
                        }

                        if (row > 0 && col == 0 && val <= conf.DownAutoSteps[row - 1][0]) good = false;
                        if (row > 0 && col == 1 && val <= conf.DownAutoSteps[row - 1][0]) good = false;
                        if (col == 0 && val <= conf.DownAutoSteps[row][1]) good = false;
                        if (col == 1 && val >= conf.DownAutoSteps[row][0]) good = false;
                    }

                    if (good)
                    {
                        DownAutorotGridView.Rows[row].Cells[col].Style.BackColor = SystemColors.Control;
                    }
                    else
                    {
                        DownAutorotGridView.Rows[row].Cells[col].Style.BackColor = System.Drawing.Color.Red;
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
        private void UpAutorotGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int val;

            if (e.RowIndex == -1) return;

            string s = UpAutorotGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
            bool good = int.TryParse(s, out val);

            if (good)
            {
                conf.UpAutoSteps[e.RowIndex][e.ColumnIndex] = val;
                conf.WriteConfig();
            }

            upErrorLabel1.Visible = check_UP_autorot_config();
            upErrorLabel2.Visible = upErrorLabel1.Visible;
        }
        private void DownAutorotGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int val;

            if (e.RowIndex == -1) return;

            string s = DownAutorotGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
            bool good = int.TryParse(s, out val);

            if (good)
            {
                conf.DownAutoSteps[e.RowIndex][e.ColumnIndex] = val;
                conf.WriteConfig();
            }

            downErrorLabel1.Visible = check_DOWN_autorot_config();
            downErrorLabel2.Visible = downErrorLabel1.Visible;
        }

        private void AutorotGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
            AutorotGridView.MaximumSize = new System.Drawing.Size(AutorotGridView.Width, stepwiseGroup.Height - 50);
            conf.WriteConfig();
            if (gr != null)
                gr.Graph_ValuesChanged();
        }

        private void AutorotGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
            AutorotGridView.MaximumSize = new System.Drawing.Size(AutorotGridView.Width, stepwiseGroup.Height - 50);
            conf.WriteConfig();
            if (gr != null)
                gr.Graph_ValuesChanged();
        }
        private void UpAutorotGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpAutorotGridView.Height = UpAutorotGridView.RowCount * 22 + 20;

            UpAutorotGridView.MaximumSize = new System.Drawing.Size(UpAutorotGridView.Width, stepwiseGroup.Height - 60);
            conf.WriteConfig();
            if (gr != null)
                gr.Graph_ValuesChanged();
        }

        private void UpAutorotGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpAutorotGridView.Height = UpAutorotGridView.RowCount * 22 + 20;
            UpAutorotGridView.MaximumSize = new System.Drawing.Size(UpAutorotGridView.Width, stepwiseGroup.Height - 60);
            conf.WriteConfig();
            if (gr != null)
                gr.Graph_ValuesChanged();
        }
        private void DownAutorotGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DownAutorotGridView.Height = DownAutorotGridView.RowCount * 22 + 20;
            DownAutorotGridView.MaximumSize = new System.Drawing.Size(DownAutorotGridView.Width, stepwiseGroup.Height - 60);
            conf.WriteConfig();
            if (gr != null)
                gr.Graph_ValuesChanged();
        }

        private void DownAutorotGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            DownAutorotGridView.Height = DownAutorotGridView.RowCount * 22 + 20;
            DownAutorotGridView.MaximumSize = new System.Drawing.Size(DownAutorotGridView.Width, stepwiseGroup.Height - 60);
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
            setButtonToolTip(AccumReset, conf.AccuResetButton);
        }

        private void SetHoldButton1_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Hold Button:", conf.HoldButton1);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Hold Button", conf.HoldButton1, conf.HoldButton2, conf.HoldButton3);
                frm.ShowDialog();
            }
            setButtonToolTip(SetHoldButton1, conf.HoldButton1);
        }

        void sizeChanged()
        {
            VersionLabel.Location = new System.Drawing.Point(VersionLabel.Location.X, Size.Height - 56);
            if (YawPitchTab.SelectedTab.Text == "Yaw")
            {
                if (conf.AutoMode == "stepwise")
                {
                    ARGroup.Height = Height - 384;
                    YawPitchTab.Height = ManualGroup.Height + ARGroup.Height + 50;
                    stepwiseGroup.Height = ARGroup.Height - 50;
                    AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
                    AutorotGridView.MaximumSize = new System.Drawing.Size(AutorotGridView.Width, stepwiseGroup.Height - 50);

                }
            }
            else 
            {
                if (conf.PitchAutoMode == "stepwise")
                {
                    pARGroup.Height = Height - 384;
                    YawPitchTab.Height = ManualGroup.Height + pARGroup.Height + 50;
                    pStepwiseGroup.Height = pARGroup.Height - 48;
                    DownAutorotGridView.Height = DownAutorotGridView.RowCount * 22 + 20;
                    DownAutorotGridView.MaximumSize = new System.Drawing.Size(DownAutorotGridView.Width, pStepwiseGroup.Height - 60);
                    UpAutorotGridView.Height = UpAutorotGridView.RowCount * 22 + 20;
                    UpAutorotGridView.MaximumSize = new System.Drawing.Size(UpAutorotGridView.Width, pStepwiseGroup.Height - 60);

                }
            }

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
            startMinimzedToolStripMenuItem.Checked = conf.StartMinimized;
            minimizeToTrayToolStripMenuItem.Checked = conf.MinimizeToTray;
            MultipleLRButtonsToolStripMenuItem.Checked = conf.MultipleLRbuttons;
            disableAllGUIOutputToolStripMenuItem.Checked = conf.DisableGUIOutput;
            disableJoystickAutoReconnectToolStripMenuItem.Checked = conf.DisableJoystickReconnect;

            ToolStripMenuItem item = (ToolStripMenuItem)PitchLimToolStripMenuItem.DropDownItems[conf.PitchLimForAutorot / 10 - 1];
            item.Checked = true;
        }

        private void resetOptionsToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conf.StartMinimized = false;
            conf.MinimizeToTray = false;
            conf.PitchLimForAutorot = 90;
            conf.DisableGUIOutput = false;
            conf.DisableJoystickReconnect = false;
            conf.MultipleLRbuttons = false;
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

            if (LayerNames[0] != "Error")
            {
                string message = "";
                string title = "OpenXR API Layers";
                foreach (string name in LayerNames)
                {
                    message = message + "\n" + name;
                }
                MessageBox.Show(message, title);
            }
        }

        private void autorot_changed(object sender, EventArgs e)
        {
            if (AROffButton.Checked)
            {
                stepwiseGroup.Visible = false;
                linearGroup.Visible = false;
                ARGroup.Height = 45;
                conf.AutoMode = "off";
                auto_offset_angle = 0;
            }
            if (ARlinear.Checked)
            {
                stepwiseGroup.Visible = false;
                linearGroup.Visible = true;
                ARGroup.Height = 140;
                linearGroup.Location = new System.Drawing.Point(7, 40);
                conf.AutoMode = "linear";
            }
            if (ARstepwise.Checked)
            {
                stepwiseGroup.Visible = true;
                linearGroup.Visible = false;
                ARGroup.Height = 247;
                stepwiseGroup.Location = new System.Drawing.Point(7, 40);
                conf.AutoMode = "stepwise";
            }
            YawPitchTab.Height = ManualGroup.Height + ARGroup.Height + 50;
            Height = YawPitchTab.Location.Y + YawPitchTab.Height + 60;
            vr.setLinearRotationSettings(conf.AutoMode == "linear", conf.LinearLimL, conf.LinearLimR,
                conf.LinearMultL, conf.LinearMultR);
            conf.WriteConfig();
        }
        private void applyLinearSettings()
        {
            vr.setLinearRotationSettings(conf.AutoMode == "linear", conf.LinearLimL, conf.LinearLimR, conf.LinearMultL, conf.LinearMultR);
            vr.setPitchLinearRotationSettings(conf.PitchAutoMode == "linear", conf.LinearLimU, conf.LinearLimD, conf.LinearMultU, conf.LinearMultD);
            conf.WriteConfig();
        }

        private void numericUpDownMultLeft_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearMultL = (int)numericUpDownMultLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultLeft_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearMultL = (int)numericUpDownMultLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultRight_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearMultR = (int)numericUpDownMultRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultRight_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearMultR = (int)numericUpDownMultRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartLeft_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearLimL = (int)numericUpDownStartLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartLeft_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearLimL = (int)numericUpDownStartLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartRight_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearLimR = (int)numericUpDownStartRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartRight_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearLimR = (int)numericUpDownStartRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartUp_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearLimU = (int)numericUpDownStartUp.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartDown_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearLimD = (int)numericUpDownStartDown.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultUp_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearMultU = (int)numericUpDownMultUp.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultDown_ValueChanged(object sender, EventArgs e)
        {
            conf.LinearMultD = (int)numericUpDownMultDown.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartUp_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearLimU = (int)numericUpDownStartUp.Value;
            applyLinearSettings();
        }
        private void numericUpDownStartDown_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearLimD = (int)numericUpDownStartDown.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultUp_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearMultU = (int)numericUpDownMultUp.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultDown_KeyUp(object sender, KeyEventArgs e)
        {
            conf.LinearMultD = (int)numericUpDownMultDown.Value;
            applyLinearSettings();
        }

        private void pitchAutorotChanged(object sender, EventArgs e)
        {
            if (pAROffButton.Checked)
            {
                pStepwiseGroup.Visible = false;
                pLinearGroup.Visible = false;
                pARGroup.Height = 45;
                conf.PitchAutoMode = "off";
                auto_offset_angle_pitch = 0;
            }
            if (pARlinear.Checked)
            {
                pStepwiseGroup.Visible = false;
                pLinearGroup.Visible = true;
                pARGroup.Height = 140;
                pLinearGroup.Location = new System.Drawing.Point(7, 40);
                conf.PitchAutoMode = "linear";
            }
            if (pARstepwise.Checked)
            {
                pStepwiseGroup.Visible = true;
                pLinearGroup.Visible = false;
                pARGroup.Height = 220;
                pStepwiseGroup.Size = new System.Drawing.Size(236, 172);
                pStepwiseGroup.Location = new System.Drawing.Point(7, 40);
                conf.PitchAutoMode = "stepwise";
            }
            YawPitchTab.Height = ManualGroup.Height + pARGroup.Height + 50;
            Height = YawPitchTab.Location.Y + YawPitchTab.Height + 60;
            vr.setPitchLinearRotationSettings(conf.PitchAutoMode == "linear", 
                conf.LinearLimU, conf.LinearLimD,
                conf.LinearMultU, conf.LinearMultD);
            conf.WriteConfig();

        }

        private void YawPitchTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (YawPitchTab.SelectedTab.Text == "Yaw")
            {
                YawPitchTab.Height = ManualGroup.Height + ARGroup.Height + 50;
            }
            else
            {
                YawPitchTab.Height = ManualGroup.Height + pARGroup.Height + 50;
            }
            Height = YawPitchTab.Location.Y + YawPitchTab.Height + 60;
        }

        private void SetDownButton_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Button for Down Rotation:", conf.DownButton);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Down", conf.DownButton, conf.DownButton2, conf.DownButton3);
                frm.ShowDialog();
            }
            setButtonToolTip(SetDownButton, conf.DownButton);
            setLabelToolTip(DownLabel, conf.DownButton);
        }

        private void SetUpButton_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Button for up Rotation:", conf.UpButton);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Down", conf.UpButton, conf.UpButton2, conf.UpButton3);
                frm.ShowDialog();
            }
            setButtonToolTip(SetUpButton, conf.UpButton);
            setLabelToolTip(UpLabel, conf.UpButton);

        }

        private void pAccumReset_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Pitch Accum Reset Button:", conf.PitchAccuResetButton);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Accum Reset", conf.PitchAccuResetButton, conf.PitchAccuResetButton2, conf.PitchAccuResetButton3);
                frm.ShowDialog();
            }
            setButtonToolTip(pAccumReset, conf.PitchAccuResetButton);
        }

        private void SetPitchHoldButton_Click(object sender, EventArgs e)
        {
            if (conf.MultipleLRbuttons == false)
            {
                ButtonForm frm = new ButtonForm(this, "Pitch Hold Button:", conf.PitchHoldButton1);
                frm.ShowDialog();
            }
            else
            {
                MultiButtons frm = new MultiButtons(this, "Hold Button", conf.PitchHoldButton1, conf.PitchHoldButton2, conf.PitchHoldButton3);
                frm.ShowDialog();
            }
            setButtonToolTip(SetPitchHoldButton, conf.PitchHoldButton1);
        }

        private void disableAllGUIOutputToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            conf.DisableGUIOutput = disableAllGUIOutputToolStripMenuItem.Checked;
            if (conf.DisableGUIOutput)
            {
                HMDYawLabel.Text = "     (HMD angle output disabled)";
                Text = "XRNS";
            }
            conf.WriteConfig();
        }

        private void disableJoystickAutoReconnectToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            conf.DisableJoystickReconnect = disableAllGUIOutputToolStripMenuItem.Checked;
            conf.WriteConfig();
            js.disableAutoReconnect = conf.DisableJoystickReconnect;
        }
    }
}
