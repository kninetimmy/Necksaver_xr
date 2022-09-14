using SharpDX;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public struct SharedMemoryData
    {
        public float hmdYawAngle;
        public float hmdPitchAngle;
        public float yawOffset;
        public float pitchOffset;
        public float lateralOffset;
        public float longitudinalOffset;
        public float rightMultiplier;
        public float leftMultiplier;
        public float upMultiplier;
        public float downMultiplier;
        public int leftStartAt;
        public int rightStartAt;
        public int upStartAt;
        public int downStartAt;
        public bool resetHmdOrientation;
        public bool useLinearRotation;
        public bool useLinearPitchRotation;
        public bool holdLinearRotation;
        public bool holdLinearPitchRotation;
        public bool hasBeenCentered;
    }

    public class VRStuff : IDisposable
    {
        private const string XRNECKSAFER_LAYER_NAME = "XR_APILAYER_NOVENDOR_XRNeckSafer";
        private const string SHARED_MEMORY_FILE_NAME = "XRNeckSaferSHM";
        private const int SHARED_MEMORY_FILE_SIZE = 80;
        private readonly MemoryMappedFile _sharedMemoryMappedFile;
        private MemoryMappedViewAccessor _memoryAccessor;
        private SharedMemoryData _sharedMemoryData;

        public VRStuff()
        {
            _sharedMemoryMappedFile = MemoryMappedFile.CreateOrOpen(SHARED_MEMORY_FILE_NAME, SHARED_MEMORY_FILE_SIZE);
            _memoryAccessor = _sharedMemoryMappedFile.CreateViewAccessor();
        }

        public unsafe List<string> ListApiLayers()
        {
            // taken from OpenXR toolkit without knowing what I'm doing
            var LayerNameList = new List<string>();
            var assemblyName = new AssemblyName();

            AppDomain dom = AppDomain.CreateDomain("temporaryXr");
            try
            {
                // Load the OpenXR package into a temporary app domain. This is so make sure that the registry is read everytime when looking for implicit API layer.

                assemblyName.CodeBase = typeof(XR).Assembly.Location;
                Assembly assembly = dom.Load(assemblyName);
                Type localXR = assembly.GetType("Silk.NET.OpenXR.XR");

                XR xr = (XR)localXR.GetMethod("GetApi").Invoke(null, null);

                // Make sure our layer is installed.
                uint layerCount = 0;
                xr.EnumerateApiLayerProperties(ref layerCount, null);
                var layers = new ApiLayerProperties[layerCount];
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].Type = StructureType.TypeApiLayerProperties;
                }
                var layersSpan = new Span<ApiLayerProperties>(layers);
                if (xr.EnumerateApiLayerProperties(ref layerCount, layersSpan) == Silk.NET.OpenXR.Result.Success)
                {
                    bool found = false;
                    for (int i = 0; i < layers.Length; i++)
                    {
                        fixed (void* nptr = layers[i].LayerName)
                        {
                            string layerName = SilkMarshal.PtrToString(new IntPtr(nptr));
                            LayerNameList.Add(layerName);
                            found = layerName.Equals(XRNECKSAFER_LAYER_NAME, StringComparison.Ordinal);
                        }
                    }
                    if (!found)
                    {
                        LayerNameList.Add("\n--> XRNeckSafer API Layer NOT active! <--");
                    }
                }
                else
                {
                    MessageBox.Show("Unable to query API layers\nUse OpenXR developer tools to \nverify layer installation", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LayerNameList.Clear();
                    LayerNameList.Add("Error");
                }
            }
            catch (Exception e)
            {
                string a = e.ToString();

                MessageBox.Show("Unable to query API layers\nUse OpenXR developer tools to \nverify layer installation\n\n" + a, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LayerNameList.Clear();
                LayerNameList.Add("Error");
            }
            finally
            {
                AppDomain.Unload(dom);
            }
            return LayerNameList;

        }

        public void ResetHmdOrientation()
        {
            _sharedMemoryData.resetHmdOrientation = true;
            _memoryAccessor.Write(0, ref _sharedMemoryData);
        }

        public void UpdateHmdOrientation()
        {
            _memoryAccessor.Read(0, out _sharedMemoryData);
        }

        public bool HmdWasCentered()
        {
            return _sharedMemoryData.hasBeenCentered;
        }

        public float GetHmdYaw()
        {
            return _sharedMemoryData.hmdYawAngle;
        }

        public float GetHmdPitch()
        {
            return _sharedMemoryData.hmdPitchAngle;
        }

        public void SetLinearRotationSettings(bool uselinear, int leftstart, int rightstart, int leftmult, int rightmult)
        {
            _sharedMemoryData.useLinearRotation = uselinear;
            _sharedMemoryData.leftStartAt = leftstart;
            _sharedMemoryData.rightStartAt = rightstart;
            _sharedMemoryData.leftMultiplier = leftmult / 100f;
            _sharedMemoryData.rightMultiplier = rightmult / 100f;
            _memoryAccessor.Write(0, ref _sharedMemoryData);
        }
        public void SetPitchLinearRotationSettings(bool usepitchlinear, int upstart, int downstart, int upmult, int downmult)
        {
            _sharedMemoryData.useLinearPitchRotation = usepitchlinear;
            _sharedMemoryData.upStartAt = upstart;
            _sharedMemoryData.downStartAt = downstart;
            _sharedMemoryData.upMultiplier = upmult / 100f;
            _sharedMemoryData.downMultiplier = downmult / 100f;
            _memoryAccessor.Write(0, ref _sharedMemoryData);
        }

        public void SetOffset(int a, int b, Vector3 trans)
        {
            _sharedMemoryData.yawOffset = (float)(a * Math.PI / 180);
            _sharedMemoryData.pitchOffset = (float)(-b * Math.PI / 180);
            _sharedMemoryData.lateralOffset = trans.X;
            _sharedMemoryData.longitudinalOffset = trans.Z;
            _memoryAccessor.Write(0, ref _sharedMemoryData);
        }

        public void SetLinearHold(bool h)
        {
            _sharedMemoryData.holdLinearRotation = h;
            _memoryAccessor.Write(0, ref _sharedMemoryData);
        }

        public void SetPitchLinearHold(bool h)
        {
            _sharedMemoryData.holdLinearRotation = h;
            _memoryAccessor.Write(0, ref _sharedMemoryData);
        }

        public void Dispose()
        {
            if (_memoryAccessor != null)
            {
                _memoryAccessor.Dispose();
                _memoryAccessor = null;
                _sharedMemoryMappedFile?.Dispose();
            }
        }
    }

    //public class Property<T>
    //{
    //    private T _value = default;

    //    public event Action<T> OnChanged;

    //    public Property(T value)
    //    {
    //        _value = value;
    //    }

    //    public Property()
    //    {
    //        _value = default;
    //    }

    //    public T Value
    //    {
    //        get => _value;
    //        set
    //        {
    //            var fireEvent = !value.Equals(_value);
    //            _value = value;
    //            if (fireEvent)
    //            {
    //                OnChanged?.Invoke(value);
    //            }
    //        }
    //    }

    //    public void Clear()
    //    {
    //        if (OnChanged != null)
    //        {
    //            foreach (var invokerDelegate in OnChanged.GetInvocationList())
    //            {
    //                OnChanged -= invokerDelegate as Action<T>;
    //            }
    //        }
    //    }
    //}

    //public class Monitor: IDisposable
    //{
    //    private readonly VRStuff _vr;
    //    private Timer _timer;

    //    private int _joyOffsetAngle;
    //    private int _autoOffsetAngle;
    //    private int _sumOffsetAngle;
    //    private int _lastOffsetAngle;
    //    private int _joyOffsetAnglePitch;
    //    private int _autoOffsetAnglePitch;
    //    private int _sumOffsetAnglePitch;
    //    private int _lastOffsetAnglePitch;
    //    private float _lastOffsetX;
    //    private float _lastOffsetZ;

    //    private float _transOffsetLeftRight;
    //    private float _transOffsetForward;
    //    private Vector3 _transOffsetVector;
    //    private Vector3 _autoTransOffsetVector;

    //    private bool _lastPressed;
    //    private bool _lastPitchPressed;
    //    private bool _lastHPressed;
    //    private bool _lastHpPressed;

    //    public RotationMode YawRotationMode
    //    {
    //        get => Config.Instance.Additiv;
    //        set
    //        {
    //            if (Config.Instance.Additiv != value)
    //            {
    //                Config.Instance.Additiv = value;
    //                Config.Instance.WriteConfig();
    //            }
    //        }
    //    }//additivRB

    //    public RotationMode PitchRotationMode
    //    {
    //        get => Config.Instance.PitchAdditiv;
    //        set
    //        {
    //            if (Config.Instance.PitchAdditiv != value)
    //            {
    //                Config.Instance.PitchAdditiv = value;
    //                Config.Instance.WriteConfig();
    //            }
    //        }
    //    } // pAdditivRB

    //    public int YawRotationAngle
    //    {
    //        get => Config.Instance.Angle;
    //        set
    //        {
    //            if (Config.Instance.Angle != value)
    //            {
    //                Config.Instance.Angle = value;
    //                Config.Instance.WriteConfig();
    //            }
    //        }
    //    } // angleNUD.Value

    //    public int PitchTiltUpAngle
    //    {
    //        get => Config.Instance.UpAngle;
    //        set
    //        {
    //            if (Config.Instance.UpAngle != value)
    //            {
    //                Config.Instance.UpAngle = value;
    //                Config.Instance.WriteConfig();
    //            }
    //        }
    //    } // upNUD.Value;

    //    public int PitchTiltDownAngle
    //    {
    //        get => Config.Instance.DownAngle;
    //        set
    //        {
    //            if (Config.Instance.DownAngle != value)
    //            {
    //                Config.Instance.DownAngle = value;
    //                Config.Instance.WriteConfig();
    //            }
    //        }
    //    } // downNUD.Value;

    //    public AutoRotationMode YawAutoRotationMode 
    //    {
    //        get => Config.Instance.AutoMode;
    //        set
    //        {
    //            if (Config.Instance.AutoMode != value)
    //            {
    //                Config.Instance.AutoMode = value;
    //                Config.Instance.WriteConfig();
    //            }
    //        }
    //    } // AROffButton.Checked, 

    //    public AutoRotationMode PitchAutoRotationMode
    //    {
    //        get => Config.Instance.PitchAutoMode;
    //        set
    //        {
    //            if (Config.Instance.PitchAutoMode != value)
    //            {
    //                Config.Instance.PitchAutoMode = value;
    //                Config.Instance.WriteConfig();
    //            }
    //        }
    //    }// pAROffButton.Checked     

    //    public Property<bool> reset_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> acc_res_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> pitch_acc_res_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> l_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> r_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> u_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> d_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> h_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<bool> hp_pressed { get; private set; } = new Property<bool>(false);
    //    public Property<float> hmdYaw { get; private set; } = new Property<float>(0);
    //    public Property<float> hmdPitch { get; private set; } = new Property<float>(0);

    //    public Monitor(VRStuff vr)
    //    {
    //        _vr = vr;
    //        _timer = new Timer
    //        {
    //            Interval = 20
    //        };
    //        _timer.Tick += UpdateCycle;
    //        ApplyLinearSettings(false);
    //        _timer.Start();
    //    }

    //    public void ApplyLinearSettings(bool saveConfig = true)
    //    {
    //        SetYawLinearRotationSettings();
    //        SetPitchLinearRotationSettings();
    //        if (saveConfig)
    //        {
    //            Config.Instance.WriteConfig();
    //        }
    //    }

    //    public void SetPitchLinearRotationSettings()
    //    {
    //        _vr.SetPitchLinearRotationSettings(Config.Instance.PitchAutoMode == AutoRotationMode.Smooth, Config.Instance.LinearLimU, Config.Instance.LinearLimD, Config.Instance.LinearMultU, Config.Instance.LinearMultD);
    //    }

    //    public void SetYawLinearRotationSettings()
    //    {
    //        _vr.SetLinearRotationSettings(Config.Instance.AutoMode == AutoRotationMode.Smooth, Config.Instance.LinearLimL, Config.Instance.LinearLimR, Config.Instance.LinearMultL, Config.Instance.LinearMultR);
    //    }

    //    public void ResetAutoOffestAnglePitch()
    //    {
    //        _autoOffsetAnglePitch = 0;
    //    }

    //    public void ResetAutoOffestAngleYaw()
    //    {
    //        _autoOffsetAngle = 0;
    //    }

    //    public List<string> ListApiLayers()
    //    {
    //        return _vr.ListApiLayers();
    //    }

    //    public void SetTransOffsetLeftRight(decimal value)
    //    {
    //        _transOffsetLeftRight = (float)value / 100F;
    //        if (Config.Instance.TransLR != (int)value)
    //        {
    //            Config.Instance.TransLR = (int)value;
    //            Config.Instance.WriteConfig();
    //        }
    //    }

    //    public void SetTransOffsetForward(decimal value)
    //    {
    //        _transOffsetForward = (float)value / 100F;
    //        if (Config.Instance.TransF != (int)value)
    //        {
    //            Config.Instance.TransF = (int)value;
    //            Config.Instance.WriteConfig();
    //        }
    //    }

    //    private void UpdateCycle(object sender, EventArgs e)
    //    {
    //        reset_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.ResetButton);
    //        acc_res_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.AccuResetButton);
    //        pitch_acc_res_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.PitchAccuResetButton);
    //        l_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.LeftButton);
    //        r_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.RightButton);
    //        u_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.UpButton);
    //        d_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.DownButton);
    //        h_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.HoldButton1);
    //        hp_pressed.Value = JoystickStuff.Instance.IsButtonPressed(Config.Instance.PitchHoldButton1);

    //        if (Config.Instance.MultipleLRbuttons)
    //        {
    //            l_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.LeftButton2);
    //            l_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.LeftButton3);
    //            r_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.RightButton2);
    //            r_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.RightButton3);
    //            u_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.UpButton2);
    //            u_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.UpButton3);
    //            d_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.DownButton2);
    //            d_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.DownButton3);
    //            reset_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.ResetButton2);
    //            reset_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.ResetButton3);
    //            acc_res_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.AccuResetButton2);
    //            acc_res_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.AccuResetButton3);
    //            pitch_acc_res_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.PitchAccuResetButton2);
    //            pitch_acc_res_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.PitchAccuResetButton3);
    //            h_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.HoldButton2);
    //            h_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.HoldButton3);
    //            hp_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.PitchHoldButton2);
    //            hp_pressed.Value |= JoystickStuff.Instance.IsButtonPressed(Config.Instance.PitchHoldButton3);
    //        }

    //        bool pitchlimit = _vr.GetHmdPitch() - 90 > Config.Instance.PitchLimForAutorot;

    //        bool autofrozen = h_pressed.Value || pitchlimit;

    //        if (h_pressed.Value != _lastHPressed)
    //        {
    //            _vr.SetLinearHold(h_pressed.Value);
    //        }
    //        _lastHPressed = h_pressed.Value;

    //        if (hp_pressed.Value != _lastHpPressed)
    //        {
    //            _vr.SetPitchLinearHold(hp_pressed.Value);
    //        }
    //        _lastHpPressed = hp_pressed.Value;

    //        _transOffsetVector = new Vector3(0, 0, 0);

    //        _vr.UpdateHmdOrientation();

    //        hmdYaw.Value = _vr.GetHmdYaw();
    //        hmdPitch.Value = -_vr.GetHmdPitch();


    //        while (hmdYaw.Value < -180) hmdYaw.Value += 360;
    //        while (hmdYaw.Value > 180) hmdYaw.Value -= 360;

    //        //if (_vr.HmdWasCentered())
    //        //{
    //        //    if (!Config.Instance.DisableGUIOutput)
    //        //    {
    //        //        _hmdtext = "HMD yaw: " + Math.Round(hmdYaw) + " deg   pitch: " + Math.Round(hmdPitch) + " deg";
    //        //    }
    //        //    else
    //        //    {
    //        //        _hmdtext = "     (HMD angle output disabled)";
    //        //    }
    //        //}
    //        //else
    //        //{
    //        //    _hmdtext = "HMD yaw: (not centered in game yet)";
    //        //}

    //        //if ((HMDYawLabel.Text != _hmdtext))
    //        //{
    //        //    HMDYawLabel.Location = new System.Drawing.Point(20, 18);
    //        //    HMDYawLabel.Text = _hmdtext;
    //        //}

    //        if (reset_pressed.Value)
    //        {
    //            _vr.ResetHmdOrientation();
    //            _joyOffsetAngle = 0;
    //            ApplyLinearSettings(false);
    //        }

    //        if (/*additivRB.Checked*/YawRotationMode == RotationMode.Accum)
    //        {
    //            if (l_pressed.Value && !_lastPressed)
    //                _joyOffsetAngle -= YawRotationAngle; //  angleNUD.Value;
    //            if (r_pressed.Value && !_lastPressed)
    //                _joyOffsetAngle += YawRotationAngle; //  angleNUD.Value;
    //            if (acc_res_pressed.Value)
    //                _joyOffsetAngle = 0;
    //        }
    //        else
    //        {
    //            if (l_pressed.Value)
    //            {
    //                _joyOffsetAngle = -YawRotationAngle; //angleNUD.Value;
    //                _transOffsetVector.X = _transOffsetLeftRight;
    //                _transOffsetVector.Z = _transOffsetForward;
    //            }
    //            else if (r_pressed.Value)
    //            {
    //                _joyOffsetAngle = YawRotationAngle; //angleNUD.Value;
    //                _transOffsetVector.X = -_transOffsetLeftRight;
    //                _transOffsetVector.Z = _transOffsetForward;
    //            }
    //            else
    //            {
    //                _joyOffsetAngle = 0;
    //                _transOffsetVector.X = 0;
    //                _transOffsetVector.Z = 0;
    //            }
    //        }

    //        if (/*pAdditivRB.Checked*/PitchRotationMode == RotationMode.Accum)
    //        {
    //            if (u_pressed.Value && !_lastPitchPressed)
    //                _joyOffsetAnglePitch += (int)PitchTiltUpAngle; //upNUD.Value;
    //            if (d_pressed.Value && !_lastPitchPressed)
    //                _joyOffsetAnglePitch += (int)PitchTiltDownAngle; // downNUD.Value;
    //            if (acc_res_pressed.Value)
    //                _joyOffsetAnglePitch = 0;
    //        }
    //        else
    //        {
    //            if (u_pressed.Value)
    //            {
    //                _joyOffsetAnglePitch = (int)PitchTiltUpAngle; // upNUD.Value;
    //            }
    //            else if (d_pressed.Value)
    //            {
    //                _joyOffsetAnglePitch = (int)-PitchTiltDownAngle; //  downNUD.Value;
    //            }
    //            else
    //            {
    //                _joyOffsetAnglePitch = 0;
    //            }
    //        }

    //        if (/*!AROffButton.Checked*/YawAutoRotationMode != AutoRotationMode.Off)
    //        {
    //            if (autofrozen)
    //            {
    //                // _ARText = "Autorotation hold";
    //                // if (pitchlimit) _ARText += " (pitch lim)";
    //                // else _ARText += " (button)";
    //            }
    //            else
    //            {
    //                // _ARText = "Autorotation";
    //                if (Config.Instance.AutoMode == AutoRotationMode.Stepwise)
    //                {
    //                    CalcAutoRotAndTrans((int)hmdYaw.Value, ref _autoOffsetAngle, ref _autoTransOffsetVector);
    //                }
    //                else
    //                {

    //                    _autoOffsetAngle = 0;
    //                    _autoTransOffsetVector.X = 0;
    //                    _autoTransOffsetVector.Y = 0;
    //                    _autoTransOffsetVector.Z = 0;
    //                }
    //            }
    //        }

    //        //if (ARGroup.Text != _ARText)
    //        //{
    //        //    ARGroup.Text = _ARText;
    //        //}

    //        if (/*!pAROffButton.Checked*/PitchAutoRotationMode != AutoRotationMode.Off)
    //        {
    //            if (hp_pressed.Value)
    //            {
    //                // _pARText = "Autorotation hold (button)";
    //            }
    //            else
    //            {
    //                // _pARText = "Autorotation";
    //                if (Config.Instance.PitchAutoMode == AutoRotationMode.Stepwise)
    //                {
    //                    CalcAutoPitch((int)hmdPitch.Value, ref _autoOffsetAnglePitch);
    //                }
    //                else
    //                {
    //                    _autoOffsetAnglePitch = 0;
    //                }
    //            }
    //        }

    //        //if (pARGroup.Text != _pARText)
    //        //{
    //        //    pARGroup.Text = _pARText;
    //        //}

    //        _sumOffsetAngle = _joyOffsetAngle + _autoOffsetAngle;
    //        _sumOffsetAnglePitch = _joyOffsetAnglePitch + _autoOffsetAnglePitch;

    //        if (Math.Abs(_autoTransOffsetVector.X) > Math.Abs(_transOffsetVector.X)) _transOffsetVector.X = _autoTransOffsetVector.X;
    //        if (Math.Abs(_autoTransOffsetVector.Z) > Math.Abs(_transOffsetVector.Z)) _transOffsetVector.Z = _autoTransOffsetVector.Z;

    //        if (_lastOffsetAngle != _sumOffsetAngle
    //            || _lastOffsetX != _transOffsetVector.X
    //            || _lastOffsetZ != _transOffsetVector.Z
    //            || _lastOffsetAnglePitch != _sumOffsetAnglePitch)
    //        {
    //            _vr.SetOffset(_sumOffsetAngle, _sumOffsetAnglePitch, _transOffsetVector);
    //            //if (!Config.Instance.DisableGUIOutput)
    //            //{
    //            //    Text = "XRNS (Y:" + _sumOffsetAngle + "  P: " + _sumOffsetAnglePitch + ")";
    //            //}
    //        }

    //        _lastPressed = l_pressed.Value || r_pressed.Value;
    //        _lastPitchPressed = u_pressed.Value || d_pressed.Value;

    //        _lastOffsetAngle = _sumOffsetAngle;
    //        _lastOffsetAnglePitch = _sumOffsetAnglePitch;
    //        _lastOffsetX = _transOffsetVector.X;
    //        _lastOffsetZ = _transOffsetVector.Z;

    //        //if (_graphForm != null)
    //        //{
    //        //    if (_graphForm.hmd != hmdYaw)
    //        //    {
    //        //        _graphForm.hmd = (int)hmdYaw;
    //        //        _graphForm.rot = (int)hmdYaw + _sumOffsetAngle;
    //        //        _graphForm.Refresh();
    //        //    }
    //        //}

    //    }

    //    private void CalcAutoRotAndTrans(int yaw, ref int arot, ref Vector3 atrans)
    //    {
    //        int yawsign = (yaw > 0) ? 1 : -1;
    //        int absyaw = yaw * yawsign;
    //        int absarot = (arot > 0) ? arot : -arot;
    //        int autorot = 0;
    //        int transx = 0;
    //        int transz = 0;

    //        int act;
    //        int deact = 0;
    //        int rot;
    //        int tx;
    //        int tz;

    //        for (int i = 0; i < Config.Instance.AutoSteps.Count; i++)
    //        {
    //            act = Config.Instance.AutoSteps[i][0];
    //            deact = Config.Instance.AutoSteps[i][1];
    //            rot = Config.Instance.AutoSteps[i][2];
    //            tx = Config.Instance.AutoSteps[i][3];
    //            tz = Config.Instance.AutoSteps[i][4];

    //            if (absyaw >= act)
    //            {
    //                autorot = rot;
    //                transx = tx;
    //                transz = tz;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }

    //        if ((absarot > autorot) && (absyaw >= deact))
    //        {
    //            return;
    //        }
    //        arot = yawsign * autorot;
    //        atrans.X = (float)transx / 100.0F * -yawsign;
    //        atrans.Z = (float)transz / 100.0F;
    //    }

    //    private void CalcAutoPitch(int pitch, ref int arot)
    //    {
    //        List<int[]> Steps;
    //        int pitchsign = (pitch > 0) ? 1 : -1;
    //        int abspitch = (pitch > 0) ? pitch : -pitch;
    //        int autorot = 0;
    //        int absarot = (arot > 0) ? arot : -arot;

    //        int act;
    //        int deact = 0;
    //        int rot;

    //        Steps = (pitch > 0) ? Config.Instance.UpAutoSteps : Config.Instance.DownAutoSteps;

    //        for (int i = 0; i < Steps.Count; i++)
    //        {
    //            act = Steps[i][0];
    //            deact = Steps[i][1];
    //            rot = Steps[i][2];

    //            if (abspitch >= act)
    //            {
    //                autorot = rot;
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }

    //        if ((absarot > autorot) && (abspitch >= deact))
    //        {
    //            return;
    //        }

    //        arot = autorot * pitchsign;
    //    }
        
    //    public void Dispose()
    //    {
    //        if (_timer != null)
    //        {
    //            reset_pressed.Clear();
    //            acc_res_pressed.Clear();
    //            pitch_acc_res_pressed.Clear();
    //            l_pressed.Clear();
    //            r_pressed.Clear();
    //            u_pressed.Clear();
    //            d_pressed.Clear();
    //            h_pressed.Clear();
    //            hp_pressed.Clear();
    //            hmdYaw.Clear();
    //            hmdPitch.Clear();
    //            _timer.Tick -= UpdateCycle;
    //            _timer.Dispose();
    //            _timer = null;
    //        }
    //    }
    //}
}
