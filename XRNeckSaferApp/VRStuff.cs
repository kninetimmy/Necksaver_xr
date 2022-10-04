using SharpDX;
using Silk.NET.Core.Native;
using Silk.NET.OpenXR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

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
                            if (layerName.Equals(XRNECKSAFER_LAYER_NAME, StringComparison.Ordinal))
                                found = true;
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

        public int GetRegistryStatus()
        {
            string k1 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Khronos\OpenXR\1\ApiLayers\Implicit";
            string k2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "XRNeckSafer", "OpenXrApiLayer", "XR_APILAYER_NOVENDOR_XRNeckSafer.json");
            return (int)Registry.GetValue(k1, k2, 99);
        }
        public void DisableApiLayer()
        {

            try
            {
               
                //find all entries for xrns. disable all, including double or outdated entries.
                List<string> regKeys = new List<string>();
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = "query HKEY_LOCAL_MACHINE\\SOFTWARE\\Khronos\\OpenXR\\1\\ApiLayers\\Implicit";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();

                        if (line.Contains("XRNeckSafer") && line.EndsWith("0x0"))
                        {
                            regKeys.Add("add HKEY_LOCAL_MACHINE\\SOFTWARE\\Khronos\\OpenXR\\1\\ApiLayers\\Implicit /v \""
                                + line.Substring(0, line.IndexOf("REG_DWORD")).Trim() + "\" /t REG_DWORD /d 1 /f");
                        }
                    }

                    process.WaitForExit();
                }

                foreach (string regKey in regKeys)
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "reg.exe";
                        process.StartInfo.Arguments = regKey;
                        process.StartInfo.Verb = "runas";
                        process.Start();
                        process.WaitForExit();
                    }
                }

                if (regKeys.Count == 0)
                {
                    MessageBox.Show("No XRNeckSafer API Layer found!", "Deactivate XRNS OpenXr Api Layer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error:" + Environment.NewLine + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        public void EnableApiLayer()
        {
            try
            {
                string regKey = "add HKEY_LOCAL_MACHINE\\SOFTWARE\\Khronos\\OpenXR\\1\\ApiLayers\\Implicit /v \""
                    + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "XRNeckSafer", "OpenXrApiLayer", "XR_APILAYER_NOVENDOR_XRNeckSafer.json")
                    + "\" /t REG_DWORD /d 0 /f";

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = regKey;
                    process.StartInfo.Verb = "runas";
                    process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + Environment.NewLine + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
