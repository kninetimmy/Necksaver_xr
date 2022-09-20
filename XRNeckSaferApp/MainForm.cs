using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace XRNeckSafer
{
    public partial class MainForm : Form
    {
        private Graph _graphForm;
        private int _joyOffsetAngle;
        private int _autoOffsetAngle;
        private int _sumOffsetAngle;
        private int _lastOffsetAngle;
        private int _joyOffsetAnglePitch;
        private int _autoOffsetAnglePitch;
        private int _sumOffsetAnglePitch;
        private int _lastOffsetAnglePitch;
        private float _lastOffsetX;
        private float _lastOffsetZ;

        private float _transOffsetLeftRight;
        private float _transOffsetForward;
        private Vector3 _transOffsetVector;
        private Vector3 _autoTransOffsetVector;

        private bool _lastPressed;
        private bool _lastPitchPressed;
        private bool _lastHPressed;
        private bool _lastHpPressed;
        private string _ARText;
        private string _pARText;
        private string _hmdtext;

        private readonly VRStuff _vr;

        public MainForm()
        {
            InitializeComponent();
            _vr = new VRStuff();
            VersionLabel.Text = GetAssemblyProductVersion();
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            JoystickService.DeviceDisconnected += OnJoystickDisconnected;

            if (Config.Instance.StartMinimized) this.WindowState = FormWindowState.Minimized;

            angleNUD.Value = Config.Instance.Angle;
            upNUD.Value = Config.Instance.UpAngle;
            downNUD.Value = Config.Instance.DownAngle;
            // transFNUP.Value = Config.Instance.TransF;
            // transLRNUP.Value = Config.Instance.TransLR;
            additivRB.Checked = Config.Instance.Additiv;
            if (Config.Instance.AutoMode == "stepwise")
            {
                ARstepwise.Checked = true;
            }
            else if (Config.Instance.AutoMode == "linear")
            {
                ARlinear.Checked = true;
            }
            else
            {
                AROffButton.Checked = true;
            }

            if (Config.Instance.PitchAutoMode == "stepwise")
            {
                pARstepwise.Checked = true;
            }
            else if (Config.Instance.PitchAutoMode == "linear")
            {
                pARlinear.Checked = true;
            }
            else
            {
                pAROffButton.Checked = true;
            }
            pitchAutorotChanged(new object(), new EventArgs());
            autorot_changed(new object(), new EventArgs());
            YawPitchTab.Height = ManualGroup.Height + ARGroup.Height + 50;
            Height = YawPitchTab.Location.Y + YawPitchTab.Height + 60;

            numericUpDownStartLeft.Value = Config.Instance.LinearLimL;
            numericUpDownStartRight.Value = Config.Instance.LinearLimR;
            numericUpDownMultLeft.Value = Config.Instance.LinearMultL;
            numericUpDownMultRight.Value = Config.Instance.LinearMultR;

            setMenuCheckmarks();

            for (int i = 0; i < Config.Instance.AutoSteps.Count; i++)
            {
                string[] r = new string[5]
                {
                    Config.Instance.AutoSteps[i][0].ToString(),
                    Config.Instance.AutoSteps[i][1].ToString(),
                    Config.Instance.AutoSteps[i][2].ToString(),
                    Config.Instance.AutoSteps[i][3].ToString(),
                    Config.Instance.AutoSteps[i][4].ToString(),
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

            for (int i = 0; i < Config.Instance.UpAutoSteps.Count; i++)
            {
                string[] r = new string[3]
                {
                    Config.Instance.UpAutoSteps[i][0].ToString(),
                    Config.Instance.UpAutoSteps[i][1].ToString(),
                    Config.Instance.UpAutoSteps[i][2].ToString(),
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

            for (int i = 0; i < Config.Instance.DownAutoSteps.Count; i++)
            {
                string[] r = new string[3]
                {
                    Config.Instance.DownAutoSteps[i][0].ToString(),
                    Config.Instance.DownAutoSteps[i][1].ToString(),
                    Config.Instance.DownAutoSteps[i][2].ToString(),
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

            // setButtonToolTip(SetLeftButton, Config.Instance.LeftButton);
            // setButtonToolTip(SetRightButton, Config.Instance.RightButton);
            // setButtonToolTip(SetResetButton, Config.Instance.ResetButton);
            setButtonToolTip(AccumReset, Config.Instance.AccuResetButton);
            // setButtonToolTip(YawAutorotationHoldButton, Config.Instance.HoldButton1);

            // setLabelToolTip(LeftLabel, Config.Instance.LeftButton);
            // setLabelToolTip(RightLabel, Config.Instance.RightButton);

            error_label.Visible = check_autorot_config();
            error_label2.Visible = error_label.Visible;
            upErrorLabel1.Visible = check_autorot_config();
            upErrorLabel2.Visible = upErrorLabel1.Visible;
            downErrorLabel1.Visible = check_autorot_config();
            downErrorLabel2.Visible = downErrorLabel1.Visible;
            numericUpDownStartLeft.Value = Config.Instance.LinearLimL;
            numericUpDownStartRight.Value = Config.Instance.LinearLimR;
            numericUpDownMultLeft.Value = Config.Instance.LinearMultL;
            numericUpDownMultRight.Value = Config.Instance.LinearMultR;
            numericUpDownStartUp.Value = Config.Instance.LinearLimU;
            numericUpDownStartDown.Value = Config.Instance.LinearLimD;
            numericUpDownMultUp.Value = Config.Instance.LinearMultU;
            numericUpDownMultDown.Value = Config.Instance.LinearMultD;

            _vr.SetLinearRotationSettings(Config.Instance.AutoMode == "linear", Config.Instance.LinearLimL, Config.Instance.LinearLimR, Config.Instance.LinearMultL, Config.Instance.LinearMultR);
            _vr.SetPitchLinearRotationSettings(Config.Instance.PitchAutoMode == "linear", Config.Instance.LinearLimU, Config.Instance.LinearLimD, Config.Instance.LinearMultU, Config.Instance.LinearMultD);
            _ARText = "Autorotation";
            _pARText = "Autorotation";
            _hmdtext = "";
            loopTimer.Start();
        }

        private void OnJoystickDisconnected(Guid guid, string joystickName)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Guid, string>(OnJoystickDisconnected), guid, joystickName);
                return;
            }
            MessageBox.Show($"Joystick {joystickName} with GUID: {guid} has been disconnected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void setButtonToolTip(Button b, ButtonConfig bc)
        {
            var text = JoystickService.GetJoystickName(bc.JoystickGUID) ?? "none" + ": " + bc.Button;
            if (bc.UseModifier)
            {
                text += "   +   " + JoystickService.GetJoystickName(bc.ModJoystickGUID) ?? "none" + ": " + bc.ModButton;
            }
            toolTip1.SetToolTip(b, text);
        }

        private static string GetAssemblyProductVersion()
        {
            return Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        private void setLabelToolTip(Label l, ButtonConfig bc)
        {
            string Text = JoystickService.GetJoystickName(bc.JoystickGUID) ?? "none" + ": " + bc.Button;
            if (bc.UseModifier)
            {
                Text += "   +   " + JoystickService.GetJoystickName(bc.ModJoystickGUID) ?? "none" + ": " + bc.ModButton;
            }
            toolTip1.SetToolTip(l, Text);
        }

        private void OnYawRotationAngleChanged(object sender, EventArgs e)
        {
            Config.Instance.Angle = (int)angleNUD.Value;
            Config.Instance.WriteConfig();
        }

        private void OnPitchTiltUpRotationChanged(object sender, EventArgs e)
        {
            Config.Instance.UpAngle = (int)upNUD.Value;
            Config.Instance.WriteConfig();
        }

        private void OnPitchTiltDownRotationChanged(object sender, EventArgs e)
        {
            Config.Instance.DownAngle = (int)downNUD.Value;
            Config.Instance.WriteConfig();
        }

        private void additivRB_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.Additiv = additivRB.Checked;
            transLRNUP.Enabled = !additivRB.Checked;
            transFNUP.Enabled = !additivRB.Checked;
            label14.Enabled = !additivRB.Checked;
            label15.Enabled = !additivRB.Checked;
            label16.Enabled = !additivRB.Checked;
            label17.Enabled = !additivRB.Checked;
            Config.Instance.WriteConfig();
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
            bool reset_pressed = SetResetButton.ActionPropertyValue;
            bool acc_res_pressed = JoystickService.IsButtonPressed(Config.Instance.AccuResetButton);
            bool pitch_acc_res_pressed = JoystickService.IsButtonPressed(Config.Instance.PitchAccuResetButton);
            bool l_pressed = SetLeftButton.ActionPropertyValue;
            bool r_pressed = SetRightButton.ActionPropertyValue;
            bool u_pressed = SetUpButton.ActionPropertyValue;
            bool d_pressed = SetDownButton.ActionPropertyValue; // JoystickService.IsButtonPressed(Config.Instance.DownButton);
            bool h_pressed = YawAutorotationHoldButton.ActionPropertyValue;
            bool hp_pressed = PitchAutorotationHoldButton.ActionPropertyValue;
            //            bool h_pressed = checkButtonPress(SetHoldButton1, conf.HoldButton1);
            if (Config.Instance.MultipleLRbuttons)
            {
                acc_res_pressed |= JoystickService.IsButtonPressed(Config.Instance.AccuResetButton2);
                acc_res_pressed |= JoystickService.IsButtonPressed(Config.Instance.AccuResetButton3);
                pitch_acc_res_pressed |= JoystickService.IsButtonPressed(Config.Instance.PitchAccuResetButton2);
                pitch_acc_res_pressed |= JoystickService.IsButtonPressed(Config.Instance.PitchAccuResetButton3);
            }

            setLabelColor(l_pressed, LeftLabel);
            setLabelColor(r_pressed, RightLabel);
            setLabelColor(u_pressed, UpLabel);
            // setButtonColor(d_pressed, SetDownButton);
            setLabelColor(d_pressed, DownLabel);
            setButtonColor(acc_res_pressed, AccumReset);
            setButtonColor(pitch_acc_res_pressed, pAccumReset);
            setButtonColor(h_pressed, YawAutorotationHoldButton);
            setButtonColor(hp_pressed, PitchAutorotationHoldButton);

            bool pitchlimit = _vr.GetHmdPitch() - 90 > Config.Instance.PitchLimForAutorot;

            bool autofrozen = h_pressed || pitchlimit;

            if (h_pressed != _lastHPressed)
            {
                _vr.SetLinearHold(h_pressed);
            }
            _lastHPressed = h_pressed;

            if (hp_pressed != _lastHpPressed)
            {
                _vr.SetPitchLinearHold(hp_pressed);
            }
            _lastHpPressed = hp_pressed;

            _transOffsetVector = new Vector3(0, 0, 0);

            _vr.UpdateHmdOrientation();

            float hmdYaw = _vr.GetHmdYaw();
            float hmdPitch = -_vr.GetHmdPitch();


            while (hmdYaw < -180) hmdYaw += 360;
            while (hmdYaw > 180) hmdYaw -= 360;

            if (_vr.HmdWasCentered())
            {
                if (!Config.Instance.DisableGUIOutput)
                {
                    _hmdtext = "HMD yaw: " + Math.Round(hmdYaw) + " deg   pitch: " + Math.Round(hmdPitch) + " deg";
                }
                else
                {
                    _hmdtext = "     (HMD angle output disabled)";
                }
            }
            else
            {
                _hmdtext = "HMD yaw: (not centered in game yet)";
            }

            if ((HMDYawLabel.Text != _hmdtext))
            {
                HMDYawLabel.Location = new System.Drawing.Point(20, 18);
                HMDYawLabel.Text = _hmdtext;
            }

            if (reset_pressed)
            {
                _vr.ResetHmdOrientation();
                _joyOffsetAngle = 0;
                _vr.SetLinearRotationSettings(Config.Instance.AutoMode == "linear", Config.Instance.LinearLimL, Config.Instance.LinearLimR, Config.Instance.LinearMultL, Config.Instance.LinearMultR);
                _vr.SetPitchLinearRotationSettings(Config.Instance.PitchAutoMode == "linear", Config.Instance.LinearLimU, Config.Instance.LinearLimD, Config.Instance.LinearMultU, Config.Instance.LinearMultD);
            }

            if (additivRB.Checked)
            {
                if (l_pressed && !_lastPressed)
                    _joyOffsetAngle -= (int)angleNUD.Value;
                if (r_pressed && !_lastPressed)
                    _joyOffsetAngle += (int)angleNUD.Value;
                if (acc_res_pressed)
                    _joyOffsetAngle = 0;
            }
            else
            {
                if (l_pressed)
                {
                    _joyOffsetAngle = -(int)angleNUD.Value;
                    _transOffsetVector.X = _transOffsetLeftRight;
                    _transOffsetVector.Z = _transOffsetForward;
                }
                else if (r_pressed)
                {
                    _joyOffsetAngle = (int)angleNUD.Value;
                    _transOffsetVector.X = -_transOffsetLeftRight;
                    _transOffsetVector.Z = _transOffsetForward;
                }
                else
                {
                    _joyOffsetAngle = 0;
                    _transOffsetVector.X = 0;
                    _transOffsetVector.Z = 0;
                }
            }

            if (pAdditivRB.Checked)
            {
                if (u_pressed && !_lastPitchPressed)
                    _joyOffsetAnglePitch += (int)upNUD.Value;
                if (d_pressed && !_lastPitchPressed)
                    _joyOffsetAnglePitch += (int)downNUD.Value;
                if (acc_res_pressed)
                    _joyOffsetAnglePitch = 0;
            }
            else
            {
                if (u_pressed)
                {
                    _joyOffsetAnglePitch = (int)upNUD.Value;
                }
                else if (d_pressed)
                {
                    _joyOffsetAnglePitch = (int)-downNUD.Value;
                }
                else
                {
                    _joyOffsetAnglePitch = 0;
                }
            }

            if (!AROffButton.Checked)
            {
                if (autofrozen)
                {
                    _ARText = "Autorotation hold";
                    if (pitchlimit) _ARText += " (pitch lim)";
                    else _ARText += " (button)";
                }
                else
                {
                    _ARText = "Autorotation";
                    if (Config.Instance.AutoMode == "stepwise")
                    {
                        calcAutoRotAndTrans((int)hmdYaw, ref _autoOffsetAngle, ref _autoTransOffsetVector);
                    }
                    else
                    {

                        _autoOffsetAngle = 0;
                        _autoTransOffsetVector.X = 0;
                        _autoTransOffsetVector.Y = 0;
                        _autoTransOffsetVector.Z = 0;
                    }
                }
            }

            if (ARGroup.Text != _ARText)
            {
                ARGroup.Text = _ARText;
            }

            if (!pAROffButton.Checked)
            {
                if (hp_pressed)
                {
                    _pARText = "Autorotation hold (button)";
                }
                else
                {
                    _pARText = "Autorotation";
                    if (Config.Instance.PitchAutoMode == "stepwise")
                    {
                        calcAutoPitch((int)hmdPitch, ref _autoOffsetAnglePitch);
                    }
                    else
                    {
                        _autoOffsetAnglePitch = 0;
                    }
                }
            }

            if (pARGroup.Text != _pARText)
            {
                pARGroup.Text = _pARText;
            }

            _sumOffsetAngle = _joyOffsetAngle + _autoOffsetAngle;
            _sumOffsetAnglePitch = _joyOffsetAnglePitch + _autoOffsetAnglePitch;

            if (Math.Abs(_autoTransOffsetVector.X) > Math.Abs(_transOffsetVector.X)) _transOffsetVector.X = _autoTransOffsetVector.X;
            if (Math.Abs(_autoTransOffsetVector.Z) > Math.Abs(_transOffsetVector.Z)) _transOffsetVector.Z = _autoTransOffsetVector.Z;

            if (_lastOffsetAngle != _sumOffsetAngle
                || _lastOffsetX != _transOffsetVector.X
                || _lastOffsetZ != _transOffsetVector.Z
                || _lastOffsetAnglePitch != _sumOffsetAnglePitch)
            {
                _vr.SetOffset(_sumOffsetAngle, _sumOffsetAnglePitch, _transOffsetVector);
                if (!Config.Instance.DisableGUIOutput)
                {
                    Text = "XRNS (Y:" + _sumOffsetAngle + "  P: " + _sumOffsetAnglePitch + ")";
                }
            }

            _lastPressed = l_pressed || r_pressed;
            _lastPitchPressed = u_pressed || d_pressed;

            _lastOffsetAngle = _sumOffsetAngle;
            _lastOffsetAnglePitch = _sumOffsetAnglePitch;
            _lastOffsetX = _transOffsetVector.X;
            _lastOffsetZ = _transOffsetVector.Z;

            if (_graphForm != null)
            {
                if (_graphForm.hmd != hmdYaw)
                {
                    _graphForm.hmd = (int)hmdYaw;
                    _graphForm.rot = (int)hmdYaw + _sumOffsetAngle;
                    _graphForm.Refresh();
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

            for (int i = 0; i < Config.Instance.AutoSteps.Count; i++)
            {
                act = Config.Instance.AutoSteps[i][0];
                deact = Config.Instance.AutoSteps[i][1];
                rot = Config.Instance.AutoSteps[i][2];
                tx = Config.Instance.AutoSteps[i][3];
                tz = Config.Instance.AutoSteps[i][4];

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

            Steps = (pitch > 0) ? Config.Instance.UpAutoSteps : Config.Instance.DownAutoSteps;

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

        private void SetTransOffsetF(decimal value)
        {
            _transOffsetForward = (float)value / 100F;
        }

        private void SetTransOffsetLR(decimal value)
        {
            _transOffsetLeftRight = (float)value / 100F;
        }

        private void OnYawForwardTranslationChanged(object sender, EventArgs e)
        {
            SetTransOffsetF(transFNUP.Value);
        }

        private void OnYawLeftRightTranslationChanged(object sender, EventArgs e)
        {
            SetTransOffsetLR(transLRNUP.Value);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            int[] i = new int[5];
            i[0] = Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count - 1][0] + 10;
            i[1] = Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count - 1][0] + 1;
            i[2] = Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count - 1][2] + 10;
            i[3] = Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count - 1][3];
            i[4] = Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count - 1][4];
            Config.Instance.AutoSteps.Add(i);
            string[] s = new string[5]
            {
                Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count-1][0].ToString(),
                Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count-1][1].ToString(),
                Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count-1][2].ToString(),
                Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count-1][3].ToString(),
                Config.Instance.AutoSteps[Config.Instance.AutoSteps.Count-1][4].ToString(),
            };
            AutorotGridView.Rows.Add(s);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (Config.Instance.AutoSteps.Count > 1)
            {
                Config.Instance.AutoSteps.RemoveAt(Config.Instance.AutoSteps.Count - 1);
                AutorotGridView.Rows.Remove(AutorotGridView.Rows[AutorotGridView.RowCount - 1]);
            }
        }
        private void UpAddButton_Click(object sender, EventArgs e)
        {
            int[] i = new int[5];
            i[0] = Config.Instance.UpAutoSteps[Config.Instance.UpAutoSteps.Count - 1][0] + 10;
            i[1] = Config.Instance.UpAutoSteps[Config.Instance.UpAutoSteps.Count - 1][0] + 1;
            i[2] = Config.Instance.UpAutoSteps[Config.Instance.UpAutoSteps.Count - 1][2] + 10;
            Config.Instance.UpAutoSteps.Add(i);
            string[] s = new string[3]
            {
                Config.Instance.UpAutoSteps[Config.Instance.UpAutoSteps.Count-1][0].ToString(),
                Config.Instance.UpAutoSteps[Config.Instance.UpAutoSteps.Count-1][1].ToString(),
                Config.Instance.UpAutoSteps[Config.Instance.UpAutoSteps.Count-1][2].ToString(),
            };
            UpAutorotGridView.Rows.Add(s);
        }

        private void UpDeleteButton_Click(object sender, EventArgs e)
        {
            if (Config.Instance.UpAutoSteps.Count > 1)
            {
                Config.Instance.UpAutoSteps.RemoveAt(Config.Instance.UpAutoSteps.Count - 1);
                UpAutorotGridView.Rows.Remove(UpAutorotGridView.Rows[UpAutorotGridView.RowCount - 1]);
            }
        }
        private void DownAddButton_Click(object sender, EventArgs e)
        {
            int[] i = new int[5];
            i[0] = Config.Instance.DownAutoSteps[Config.Instance.DownAutoSteps.Count - 1][0] + 10;
            i[1] = Config.Instance.DownAutoSteps[Config.Instance.DownAutoSteps.Count - 1][0] + 1;
            i[2] = Config.Instance.DownAutoSteps[Config.Instance.DownAutoSteps.Count - 1][2] + 10;
            Config.Instance.DownAutoSteps.Add(i);
            string[] s = new string[3]
            {
                Config.Instance.DownAutoSteps[Config.Instance.DownAutoSteps.Count-1][0].ToString(),
                Config.Instance.DownAutoSteps[Config.Instance.DownAutoSteps.Count-1][1].ToString(),
                Config.Instance.DownAutoSteps[Config.Instance.DownAutoSteps.Count-1][2].ToString(),
            };
            DownAutorotGridView.Rows.Add(s);
        }

        private void DownDeleteButton_Click(object sender, EventArgs e)
        {
            if (Config.Instance.DownAutoSteps.Count > 1)
            {
                Config.Instance.DownAutoSteps.RemoveAt(Config.Instance.DownAutoSteps.Count - 1);
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
                            if (val >= Config.Instance.AutoSteps[row + 1][1]) good = false;
                            if (val >= Config.Instance.AutoSteps[row + 1][0]) good = false;
                        }

                        if (row > 0 && col == 0 && val <= Config.Instance.AutoSteps[row - 1][0]) good = false;
                        if (row > 0 && col == 1 && val <= Config.Instance.AutoSteps[row - 1][0]) good = false;
                        if (col == 0 && val <= Config.Instance.AutoSteps[row][1]) good = false;
                        if (col == 1 && val >= Config.Instance.AutoSteps[row][0]) good = false;
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
                            if (val >= Config.Instance.UpAutoSteps[row + 1][1]) good = false;
                            if (val >= Config.Instance.UpAutoSteps[row + 1][0]) good = false;
                        }

                        if (row > 0 && col == 0 && val <= Config.Instance.UpAutoSteps[row - 1][0]) good = false;
                        if (row > 0 && col == 1 && val <= Config.Instance.UpAutoSteps[row - 1][0]) good = false;
                        if (col == 0 && val <= Config.Instance.UpAutoSteps[row][1]) good = false;
                        if (col == 1 && val >= Config.Instance.UpAutoSteps[row][0]) good = false;
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
                            if (val >= Config.Instance.DownAutoSteps[row + 1][1]) good = false;
                            if (val >= Config.Instance.DownAutoSteps[row + 1][0]) good = false;
                        }

                        if (row > 0 && col == 0 && val <= Config.Instance.DownAutoSteps[row - 1][0]) good = false;
                        if (row > 0 && col == 1 && val <= Config.Instance.DownAutoSteps[row - 1][0]) good = false;
                        if (col == 0 && val <= Config.Instance.DownAutoSteps[row][1]) good = false;
                        if (col == 1 && val >= Config.Instance.DownAutoSteps[row][0]) good = false;
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
                Config.Instance.AutoSteps[e.RowIndex][e.ColumnIndex] = val;
                Config.Instance.WriteConfig();
            }

            error_label.Visible = check_autorot_config();
            error_label2.Visible = error_label.Visible;
            if (_graphForm != null)
                _graphForm.Graph_ValuesChanged();
        }
        private void UpAutorotGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int val;

            if (e.RowIndex == -1) return;

            string s = UpAutorotGridView[e.ColumnIndex, e.RowIndex].Value.ToString();
            bool good = int.TryParse(s, out val);

            if (good)
            {
                Config.Instance.UpAutoSteps[e.RowIndex][e.ColumnIndex] = val;
                Config.Instance.WriteConfig();
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
                Config.Instance.DownAutoSteps[e.RowIndex][e.ColumnIndex] = val;
                Config.Instance.WriteConfig();
            }

            downErrorLabel1.Visible = check_DOWN_autorot_config();
            downErrorLabel2.Visible = downErrorLabel1.Visible;
        }

        private void AutorotGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
            AutorotGridView.MaximumSize = new Size(AutorotGridView.Width, stepwiseGroup.Height - 50);
            Config.Instance.WriteConfig();
            if (_graphForm != null)
                _graphForm.Graph_ValuesChanged();
        }

        private void AutorotGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
            AutorotGridView.MaximumSize = new Size(AutorotGridView.Width, stepwiseGroup.Height - 50);
            Config.Instance.WriteConfig();
            if (_graphForm != null)
                _graphForm.Graph_ValuesChanged();
        }
        private void UpAutorotGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpAutorotGridView.Height = UpAutorotGridView.RowCount * 22 + 20;

            UpAutorotGridView.MaximumSize = new Size(UpAutorotGridView.Width, stepwiseGroup.Height - 60);
            Config.Instance.WriteConfig();
            if (_graphForm != null)
                _graphForm.Graph_ValuesChanged();
        }

        private void UpAutorotGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpAutorotGridView.Height = UpAutorotGridView.RowCount * 22 + 20;
            UpAutorotGridView.MaximumSize = new Size(UpAutorotGridView.Width, stepwiseGroup.Height - 60);
            Config.Instance.WriteConfig();
            if (_graphForm != null)
                _graphForm.Graph_ValuesChanged();
        }
        private void DownAutorotGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DownAutorotGridView.Height = DownAutorotGridView.RowCount * 22 + 20;
            DownAutorotGridView.MaximumSize = new Size(DownAutorotGridView.Width, stepwiseGroup.Height - 60);
            Config.Instance.WriteConfig();
            if (_graphForm != null)
                _graphForm.Graph_ValuesChanged();
        }

        private void DownAutorotGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            DownAutorotGridView.Height = DownAutorotGridView.RowCount * 22 + 20;
            DownAutorotGridView.MaximumSize = new Size(DownAutorotGridView.Width, stepwiseGroup.Height - 60);
            Config.Instance.WriteConfig();
            if (_graphForm != null)
                _graphForm.Graph_ValuesChanged();
        }

        private void startMinimzedToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Config.Instance.StartMinimized = startMinimzedToolStripMenuItem.Checked;
            Config.Instance.WriteConfig();
        }

        private void minimizeToTrayToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Config.Instance.MinimizeToTray = minimizeToTrayToolStripMenuItem.Checked;
            Config.Instance.WriteConfig();
        }

        private void SetLeftButtonClick(object sender, EventArgs e)
        {
            var button = (BooleanActionButton)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        private void SetRightButtonClick(object sender, EventArgs e)
        {
            var button = (BooleanActionButton)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        private void SetResetButton_Click(object sender, EventArgs e)
        {
            var button = (BooleanActionButton)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        private void AccumReset_Click(object sender, EventArgs e)
        {
            if (!Config.Instance.MultipleLRbuttons)
            {
                ButtonForm.Show(Top, Right, "Accum Reset Button:", Config.Instance.AccuResetButton);
            }
            else
            {
                MultiButtons.Show(Top, Right, "Accum Reset", Config.Instance.AccuResetButton, Config.Instance.AccuResetButton2, Config.Instance.AccuResetButton3);
            }
            setButtonToolTip(AccumReset, Config.Instance.AccuResetButton);
        }

        private void YawAutorotationHoldButtonClick(object sender, EventArgs e)
        {
            var button = (BooleanActionButton)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        void sizeChanged()
        {
            VersionLabel.Location = new System.Drawing.Point(VersionLabel.Location.X, Size.Height - 56);
            if (YawPitchTab.SelectedTab.Text == "Yaw")
            {
                if (Config.Instance.AutoMode == "stepwise")
                {
                    ARGroup.Height = Height - 384;
                    YawPitchTab.Height = ManualGroup.Height + ARGroup.Height + 50;
                    stepwiseGroup.Height = ARGroup.Height - 50;
                    AutorotGridView.Height = AutorotGridView.RowCount * 22 + 20;
                    AutorotGridView.MaximumSize = new Size(AutorotGridView.Width, stepwiseGroup.Height - 50);

                }
            }
            else
            {
                if (Config.Instance.PitchAutoMode == "stepwise")
                {
                    pARGroup.Height = Height - 384;
                    YawPitchTab.Height = ManualGroup.Height + pARGroup.Height + 50;
                    pStepwiseGroup.Height = pARGroup.Height - 48;
                    DownAutorotGridView.Height = DownAutorotGridView.RowCount * 22 + 20;
                    DownAutorotGridView.MaximumSize = new Size(DownAutorotGridView.Width, pStepwiseGroup.Height - 60);
                    UpAutorotGridView.Height = UpAutorotGridView.RowCount * 22 + 20;
                    UpAutorotGridView.MaximumSize = new Size(UpAutorotGridView.Width, pStepwiseGroup.Height - 60);

                }
            }

        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            sizeChanged();
        }

        private void sendToTrayIfNeeded()
        {
            if (Config.Instance.MinimizeToTray)
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
            if (Config.Instance.MinimizeToTray && Config.Instance.StartMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }

        private void setMenuCheckmarks()
        {
            startMinimzedToolStripMenuItem.Checked = Config.Instance.StartMinimized;
            minimizeToTrayToolStripMenuItem.Checked = Config.Instance.MinimizeToTray;
            MultipleLRButtonsToolStripMenuItem.Checked = Config.Instance.MultipleLRbuttons;
            disableAllGUIOutputToolStripMenuItem.Checked = Config.Instance.DisableGUIOutput;
            disableJoystickAutoReconnectToolStripMenuItem.Checked = Config.Instance.DisableJoystickReconnect;

            ToolStripMenuItem item = (ToolStripMenuItem)PitchLimToolStripMenuItem.DropDownItems[Config.Instance.PitchLimForAutorot / 10 - 1];
            item.Checked = true;
        }

        private void resetOptionsToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.Instance.StartMinimized = false;
            Config.Instance.MinimizeToTray = false;
            Config.Instance.PitchLimForAutorot = 90;
            Config.Instance.DisableGUIOutput = false;
            Config.Instance.DisableJoystickReconnect = false;
            Config.Instance.MultipleLRbuttons = false;
            Config.Instance.WriteConfig();
            setMenuCheckmarks();
        }

        private void graphButton_Click(object sender, EventArgs e)
        {
            _graphForm = new Graph(Top, Right);
            _graphForm.Show();
        }

        private void PitchLimToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach (ToolStripMenuItem item in PitchLimToolStripMenuItem.DropDownItems) item.Checked = false;
            ((ToolStripMenuItem)e.ClickedItem).Checked = true;
            int.TryParse(e.ClickedItem.Text.Substring(0, 2), out Config.Instance.PitchLimForAutorot);
            Config.Instance.WriteConfig();
        }

        private void moreLRButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MultipleLRButtonsToolStripMenuItem.Checked)
            {
                MultipleLRButtonsToolStripMenuItem.Checked = false;
                Config.Instance.MultipleLRbuttons = false;
                Config.Instance.WriteConfig();
            }
            else
            {
                MultipleLRButtonsToolStripMenuItem.Checked = true;
                Config.Instance.MultipleLRbuttons = true;
                Config.Instance.WriteConfig();
            }
        }

        private void listApiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> LayerNames = _vr.ListApiLayers();

            if (LayerNames[0] != "Error")
            {
                string message = "";
                foreach (string name in LayerNames)
                {
                    message = message + "\n" + name;
                }
                MessageBox.Show(message, "OpenXR API Layers");
            }
        }

        private void autorot_changed(object sender, EventArgs e)
        {
            if (AROffButton.Checked)
            {
                stepwiseGroup.Visible = false;
                linearGroup.Visible = false;
                ARGroup.Height = 45;
                Config.Instance.AutoMode = "off";
                _autoOffsetAngle = 0;
            }
            if (ARlinear.Checked)
            {
                stepwiseGroup.Visible = false;
                linearGroup.Visible = true;
                ARGroup.Height = 140;
                linearGroup.Location = new System.Drawing.Point(7, 40);
                Config.Instance.AutoMode = "linear";
            }
            if (ARstepwise.Checked)
            {
                stepwiseGroup.Visible = true;
                linearGroup.Visible = false;
                ARGroup.Height = 247;
                stepwiseGroup.Location = new System.Drawing.Point(7, 40);
                Config.Instance.AutoMode = "stepwise";
            }
            YawPitchTab.Height = ManualGroup.Height + ARGroup.Height + 50;
            Height = YawPitchTab.Location.Y + YawPitchTab.Height + 60;
            _vr.SetLinearRotationSettings(Config.Instance.AutoMode == "linear", Config.Instance.LinearLimL, Config.Instance.LinearLimR,
                Config.Instance.LinearMultL, Config.Instance.LinearMultR);
            Config.Instance.WriteConfig();
        }
        private void applyLinearSettings()
        {
            _vr.SetLinearRotationSettings(Config.Instance.AutoMode == "linear", Config.Instance.LinearLimL, Config.Instance.LinearLimR, Config.Instance.LinearMultL, Config.Instance.LinearMultR);
            _vr.SetPitchLinearRotationSettings(Config.Instance.PitchAutoMode == "linear", Config.Instance.LinearLimU, Config.Instance.LinearLimD, Config.Instance.LinearMultU, Config.Instance.LinearMultD);
            Config.Instance.WriteConfig();
        }

        private void numericUpDownMultLeft_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearMultL = (int)numericUpDownMultLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultLeft_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearMultL = (int)numericUpDownMultLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultRight_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearMultR = (int)numericUpDownMultRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultRight_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearMultR = (int)numericUpDownMultRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartLeft_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearLimL = (int)numericUpDownStartLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartLeft_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearLimL = (int)numericUpDownStartLeft.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartRight_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearLimR = (int)numericUpDownStartRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartRight_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearLimR = (int)numericUpDownStartRight.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartUp_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearLimU = (int)numericUpDownStartUp.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartDown_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearLimD = (int)numericUpDownStartDown.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultUp_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearMultU = (int)numericUpDownMultUp.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultDown_ValueChanged(object sender, EventArgs e)
        {
            Config.Instance.LinearMultD = (int)numericUpDownMultDown.Value;
            applyLinearSettings();
        }

        private void numericUpDownStartUp_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearLimU = (int)numericUpDownStartUp.Value;
            applyLinearSettings();
        }
        private void numericUpDownStartDown_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearLimD = (int)numericUpDownStartDown.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultUp_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearMultU = (int)numericUpDownMultUp.Value;
            applyLinearSettings();
        }

        private void numericUpDownMultDown_KeyUp(object sender, KeyEventArgs e)
        {
            Config.Instance.LinearMultD = (int)numericUpDownMultDown.Value;
            applyLinearSettings();
        }

        private void pitchAutorotChanged(object sender, EventArgs e)
        {
            if (pAROffButton.Checked)
            {
                pStepwiseGroup.Visible = false;
                pLinearGroup.Visible = false;
                pARGroup.Height = 45;
                Config.Instance.PitchAutoMode = "off";
                _autoOffsetAnglePitch = 0;
            }
            if (pARlinear.Checked)
            {
                pStepwiseGroup.Visible = false;
                pLinearGroup.Visible = true;
                pARGroup.Height = 140;
                pLinearGroup.Location = new System.Drawing.Point(7, 40);
                Config.Instance.PitchAutoMode = "linear";
            }
            if (pARstepwise.Checked)
            {
                pStepwiseGroup.Visible = true;
                pLinearGroup.Visible = false;
                pARGroup.Height = 220;
                pStepwiseGroup.Size = new Size(236, 172);
                pStepwiseGroup.Location = new System.Drawing.Point(7, 40);
                Config.Instance.PitchAutoMode = "stepwise";
            }
            YawPitchTab.Height = ManualGroup.Height + pARGroup.Height + 50;
            Height = YawPitchTab.Location.Y + YawPitchTab.Height + 60;
            _vr.SetPitchLinearRotationSettings(Config.Instance.PitchAutoMode == "linear",
                Config.Instance.LinearLimU, Config.Instance.LinearLimD,
                Config.Instance.LinearMultU, Config.Instance.LinearMultD);
            Config.Instance.WriteConfig();

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

        private void OnSetDownClick(object sender, EventArgs e)
        {
            var button = (BooleanActionButton)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        private void OnSetUpClick(object sender, EventArgs e)
        {
            var button = (BooleanActionButton)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        private void pAccumReset_Click(object sender, EventArgs e)
        {
            if (!Config.Instance.MultipleLRbuttons)
            {
                ButtonForm.Show(Top, Right, "Pitch Accum Reset Button:", Config.Instance.PitchAccuResetButton);
            }
            else
            {
                MultiButtons.Show(Top, Right, "Accum Reset", Config.Instance.PitchAccuResetButton, Config.Instance.PitchAccuResetButton2, Config.Instance.PitchAccuResetButton3);
            }
            setButtonToolTip(pAccumReset, Config.Instance.PitchAccuResetButton);
        }

        private void PitchAutorotationHoldButtonClick(object sender, EventArgs e)
        {
            var button = (BooleanActionButton)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        private void disableAllGUIOutputToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Config.Instance.DisableGUIOutput = disableAllGUIOutputToolStripMenuItem.Checked;
            if (Config.Instance.DisableGUIOutput)
            {
                HMDYawLabel.Text = "     (HMD angle output disabled)";
                Text = "XRNS";
            }
            Config.Instance.WriteConfig();
        }

        private void disableJoystickAutoReconnectToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Config.Instance.WriteConfig();
        }

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void OnTranslationLeftRightDoubleClick(object sender, MouseEventArgs e)
        {
            var button = (NumericActionUpDown)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        private void OnTranslationForwardDoubleClick(object sender, MouseEventArgs e)
        {
            var button = (NumericActionUpDown)sender;
            ActionPropertiesForm.ShowForm(button.ActionPropertyName, Top, Right);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // disable navigation on form using keyboard in order to avoid
            // mess up with actions which have these keys assigned
            if (!msg.HWnd.Equals(Handle) &&
                (keyData == Keys.Left
                || keyData == Keys.Right
                || keyData == Keys.Up
                || keyData == Keys.Down
                || keyData == Keys.Tab
                || keyData == Keys.Space
                || keyData == Keys.Enter))
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnFormLoaded(object sender, EventArgs e)
        {
            var splashScreen = new Wpf.SplashScreen();
            ElementHost.EnableModelessKeyboardInterop(splashScreen);
            splashScreen.ShowDialog();
        }
    }
}
