using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Linq;

namespace XRNeckSafer
{
    public static class RegistryService
    {
        private const string SUB_KEY = @"SOFTWARE\Khronos\OpenXR\1\ApiLayers\Implicit";
        private static readonly string _fullKey = $"{Registry.LocalMachine}\\{SUB_KEY}";
        private static readonly string _jsonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 
            "XRNeckSafer", "OpenXrApiLayer", "XR_APILAYER_NOVENDOR_XRNeckSafer.json");

        public static int? GetRegistryStatus()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(SUB_KEY))
            {
                if (key == null)
                {
                    return null;
                }
                var name = key.GetValueNames().FirstOrDefault(n => _jsonPath.Equals(n));
                if (name == null)
                {
                    return null;
                }
                var value = key.GetValue(name);
                if (!int.TryParse(value.ToString(), out int result))
                {
                    return null;
                }
                return result;
            }
        }

        public static void DisableApiLayer()
        {
            try
            {
                var regKeys = new List<string>();
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "reg.exe";
                    process.StartInfo.Arguments = $"query {_fullKey}";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();

                        if (line.Contains("XRNeckSafer") && line.EndsWith("0x0"))
                        {
                            regKeys.Add($"add {_fullKey} /v \""
                                + line.Substring(0, line.IndexOf("REG_DWORD")).Trim() + "\" /t REG_DWORD /d 1 /f");
                        }
                    }

                    process.WaitForExit();
                }

                foreach (string regKey in regKeys)
                {
                    using (var process = new Process())
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

        public static void EnableApiLayer()
        {
            try
            {
                string regKey = $"add {_fullKey} /v \""
                    + _jsonPath
                    + "\" /t REG_DWORD /d 0 /f";

                using (var process = new Process())
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
