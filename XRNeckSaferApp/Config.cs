using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XRNeckSafer
{
    public enum AutoMode
    {
        Off,
        Linear,
        Stepwise
    }

    public class Config
    {
        public bool Additiv;
        public bool PitchAdditiv;
        public bool StartMinimized;
        public bool MinimizeToTray;
        public bool DisableSplashScreen;
        public bool DisableGUIOutput;
        public int PitchLimForAutorot;
        public static string configfilename;
        public AutoMode AutoMode;
        public List<int[]> AutoSteps;
        public AutoMode PitchAutoMode;
        public List<int[]> UpAutoSteps;
        public List<int[]> DownAutoSteps;

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
        public List<ActionProperty> ActionProperties { get; set; }

        public List<KeyboardToJoystickModel> KeyboardToJoystickAssignments { get; set; }

        public static event Action ConfigReloaded;

        private Config()
        {
            Additiv = false;
            PitchAdditiv = false;
            StartMinimized = false;
            MinimizeToTray = false;
            DisableGUIOutput = false;
            DisableSplashScreen = false;
            PitchLimForAutorot = 90;
            AutoMode = AutoMode.Off;
            AutoSteps = new List<int[]>();
            PitchAutoMode = AutoMode.Off;
            UpAutoSteps = new List<int[]>();
            DownAutoSteps = new List<int[]>();
        }

        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ReadConfig();
                }
                return _instance;
            }
        }

        public static Config ReloadConfig()
        {
            _instance = ReadConfig();
            ConfigReloaded?.Invoke();
            return _instance;
        }

        private static Config ReadConfig()
        {
            try
            {
                configfilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "XRNeckSafer", "XRNeckSafer.cfg");
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                    configfilename = @".\" + args[1];

                if (!File.Exists(configfilename))
                {
                    return CreateDefaultConfig();
                }
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configfilename), new Newtonsoft.Json.Converters.StringEnumConverter());
                if (config.ActionProperties == null) config.ActionProperties = new List<ActionProperty>();
                if (config.KeyboardToJoystickAssignments == null) config.KeyboardToJoystickAssignments = new List<KeyboardToJoystickModel>();

                if (config.AutoSteps.Count == 0)
                {
                    config.AutoSteps.Add(new int[5] { 60, 51, 10, 0, 0 });
                    config.AutoSteps.Add(new int[5] { 70, 61, 20, 5, 1 });
                    config.AutoSteps.Add(new int[5] { 80, 71, 30, 7, 3 });
                    config.AutoSteps.Add(new int[5] { 90, 81, 40, 10, 5 });
                    config.AutoSteps.Add(new int[5] { 100, 91, 50, 10, 5 });
                    config.AutoSteps.Add(new int[5] { 110, 101, 60, 10, 5 });
                    config.AutoSteps.Add(new int[5] { 120, 111, 70, 10, 5 });
                }
                if (config.UpAutoSteps.Count == 0)
                {
                    config.UpAutoSteps.Add(new int[3] { 50, 41, 10 });
                    config.UpAutoSteps.Add(new int[3] { 60, 51, 20 });
                    config.UpAutoSteps.Add(new int[3] { 70, 61, 30 });
                    config.UpAutoSteps.Add(new int[3] { 80, 71, 40 });
                }
                if (config.DownAutoSteps.Count == 0)
                {
                    config.DownAutoSteps.Add(new int[3] { 50, 41, 10 });
                    config.DownAutoSteps.Add(new int[3] { 60, 51, 20 });
                    config.DownAutoSteps.Add(new int[3] { 70, 61, 30 });
                    config.DownAutoSteps.Add(new int[3] { 80, 71, 40 });
                }

                MigrateToMultibuttonMode(config);

                return config;
            }
            catch (Exception)
            {
                return CreateDefaultConfig();
            }
        }

        [Obsolete("Converts old Keyboard-To-Joystick assignments config to a new multibutton format. This should be removed in a new version")]
        private static void MigrateToMultibuttonMode(Config config)
        {
            config.KeyboardToJoystickAssignments.ForEach(x =>
            {
                if (x.KeyboardKey != System.Windows.Forms.Keys.None)
                {
                    x.KeyboardKeys = x.KeyboardKeys ?? new KeyboardKeys();
                    if (!x.KeyboardKeys.Contains(x.KeyboardKey))
                    {
                        x.KeyboardKeys.Add(x.KeyboardKey);
                    }
                    x.KeyboardKey = System.Windows.Forms.Keys.None;
                }
                if (x.JoystickButton != null)
                {
                    x.JoystickButtons = x.JoystickButtons ?? new JoystickButtons();
                    if (!x.JoystickButtons.Any(b => b.GetId() == x.JoystickButton.GetId()))
                    {
                        x.JoystickButtons.Add(x.JoystickButton);
                    }
                    x.JoystickButton = null;
                }
            });
        }

        private static Config CreateDefaultConfig()
        {
            Config conf = new Config();
            if (conf.AutoSteps.Count == 0)
            {
                conf.AutoSteps.Add(new int[5] { 60, 51, 10, 0, 0 });
                conf.AutoSteps.Add(new int[5] { 70, 61, 20, 5, 1 });
                conf.AutoSteps.Add(new int[5] { 80, 71, 30, 7, 3 });
                conf.AutoSteps.Add(new int[5] { 90, 81, 40, 10, 5 });
                conf.AutoSteps.Add(new int[5] { 100, 91, 50, 10, 5 });
                conf.AutoSteps.Add(new int[5] { 110, 101, 60, 10, 5 });
                conf.AutoSteps.Add(new int[5] { 120, 111, 70, 10, 5 });
            }
            if (conf.UpAutoSteps.Count == 0)
            {
                conf.UpAutoSteps.Add(new int[3] { 50, 41, 10 });
                conf.UpAutoSteps.Add(new int[3] { 60, 51, 20 });
                conf.UpAutoSteps.Add(new int[3] { 70, 61, 30 });
                conf.UpAutoSteps.Add(new int[3] { 80, 71, 40 });
            }
            if (conf.DownAutoSteps.Count == 0)
            {
                conf.DownAutoSteps.Add(new int[3] { 50, 41, 10 });
                conf.DownAutoSteps.Add(new int[3] { 60, 51, 20 });
                conf.DownAutoSteps.Add(new int[3] { 70, 61, 30 });
                conf.DownAutoSteps.Add(new int[3] { 80, 71, 40 });
            }
            if (conf.ActionProperties == null) conf.ActionProperties = new List<ActionProperty>();
            return conf;
        }

        public void WriteConfig()
        {
            var directory = Path.GetDirectoryName(configfilename);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(configfilename, JsonConvert.SerializeObject(this, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter()));
        }
    }
}
