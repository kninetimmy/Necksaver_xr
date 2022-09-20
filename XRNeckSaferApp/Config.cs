using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace XRNeckSafer
{
    public class ButtonConfig
    {
        public string JoystickGUID;
        public string Button;
        public string ModJoystickGUID;
        public string ModButton;
        public bool UseModifier;
        public bool Use8WayHat;
        public bool Invert;
        public bool Toggle;
        [JsonIgnore]
        public bool togglestate;
        public bool laststate;
        public ButtonConfig()
        {
            JoystickGUID = "none";
            Button = "none";
            ModJoystickGUID = "none";
            ModButton = "none";
            UseModifier = false;
            Use8WayHat = false;
            Invert = false;
            Toggle = false;
            togglestate = false;
            laststate = false;
        }

        public ButtonConfig CopyConfig(ButtonConfig buttonConfig)
        {
            buttonConfig.JoystickGUID = string.Copy(JoystickGUID);
            buttonConfig.Button = string.Copy(Button);
            buttonConfig.ModJoystickGUID = string.Copy(ModJoystickGUID);
            buttonConfig.ModButton = string.Copy(ModButton);
            buttonConfig.UseModifier = UseModifier;
            buttonConfig.Use8WayHat = Use8WayHat;
            buttonConfig.Invert = Invert;
            buttonConfig.Toggle = Toggle;

            return buttonConfig;
        }

    }

    public class Config
    {
        public ButtonConfig AccuResetButton;
        public ButtonConfig AccuResetButton2;
        public ButtonConfig AccuResetButton3;
        public ButtonConfig UpButton;
        public ButtonConfig UpButton2;
        public ButtonConfig UpButton3;
        public ButtonConfig DownButton;
        public ButtonConfig DownButton2;
        public ButtonConfig DownButton3;
        public ButtonConfig PitchAccuResetButton;
        public ButtonConfig PitchAccuResetButton2;
        public ButtonConfig PitchAccuResetButton3;
        public int Angle;
        public int UpAngle;
        public int DownAngle;
        // public int TransLR; // replaced with ActionProperty
        // public int TransF; // replaced with ActionProperty
        public int LinearLimL;
        public int LinearLimR;
        public int LinearMultL;
        public int LinearMultR;
        public int LinearLimU;
        public int LinearLimD;
        public int LinearMultU;
        public int LinearMultD;
        public bool Additiv;
        public bool PitchAdditiv;
        public bool StartMinimized;
        public bool MinimizeToTray;
        public bool MultipleLRbuttons;
        public bool DisableGUIOutput;
        public bool DisableJoystickReconnect;
        public int PitchLimForAutorot;
        public static string configfilename;
        public string AutoMode;
        public List<int[]> AutoSteps;
        public string PitchAutoMode;
        public List<int[]> UpAutoSteps;
        public List<int[]> DownAutoSteps;

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
        public List<ActionProperty> ActionProperties { get; set; }

        public static event Action ConfigReloaded;

        private Config()
        {
            AccuResetButton = new ButtonConfig();
            AccuResetButton2 = new ButtonConfig();
            AccuResetButton3 = new ButtonConfig();
            UpButton = new ButtonConfig();
            UpButton2 = new ButtonConfig();
            UpButton3 = new ButtonConfig();
            DownButton = new ButtonConfig();
            DownButton2 = new ButtonConfig();
            DownButton3 = new ButtonConfig();
            PitchAccuResetButton = new ButtonConfig();
            PitchAccuResetButton2 = new ButtonConfig();
            PitchAccuResetButton3 = new ButtonConfig();
            Angle = 30;
            UpAngle = 30;
            DownAngle = 30;
            //TransLR = 0;
            //TransF = 0;
            LinearLimL = 95;
            LinearLimR = 95;
            LinearMultL = 120;
            LinearMultR = 120;
            LinearLimU = 45;
            LinearLimD = 30;
            LinearMultU = 120;
            LinearMultD = 120;
            Additiv = false;
            PitchAdditiv = false;
            StartMinimized = false;
            MinimizeToTray = false;
            MultipleLRbuttons = false;
            DisableGUIOutput = false;
            DisableJoystickReconnect = false;
            PitchLimForAutorot = 90;
            AutoMode = "Off";
            AutoSteps = new List<int[]>();
            PitchAutoMode = "Off";
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
                Config c = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configfilename), new Newtonsoft.Json.Converters.StringEnumConverter());
                if (c.AccuResetButton == null) c.AccuResetButton = new ButtonConfig();
                if (c.AccuResetButton2 == null) c.AccuResetButton2 = new ButtonConfig();
                if (c.AccuResetButton3 == null) c.AccuResetButton3 = new ButtonConfig();
                if (c.UpButton == null) c.UpButton = new ButtonConfig();
                if (c.UpButton2 == null) c.UpButton2 = new ButtonConfig();
                if (c.UpButton3 == null) c.UpButton3 = new ButtonConfig();
                if (c.DownButton == null) c.DownButton = new ButtonConfig();
                if (c.DownButton2 == null) c.DownButton2 = new ButtonConfig();
                if (c.DownButton3 == null) c.DownButton3 = new ButtonConfig();
                if (c.PitchAccuResetButton == null) c.PitchAccuResetButton = new ButtonConfig();
                if (c.PitchAccuResetButton2 == null) c.PitchAccuResetButton2 = new ButtonConfig();
                if (c.PitchAccuResetButton3 == null) c.PitchAccuResetButton3 = new ButtonConfig();
                if (c.ActionProperties == null) c.ActionProperties = new List<ActionProperty>();

                if (c.AutoSteps.Count == 0)
                {
                    c.AutoSteps.Add(new int[5] { 60, 51, 10, 0, 0 });
                    c.AutoSteps.Add(new int[5] { 70, 61, 20, 5, 1 });
                    c.AutoSteps.Add(new int[5] { 80, 71, 30, 7, 3 });
                    c.AutoSteps.Add(new int[5] { 90, 81, 40, 10, 5 });
                    c.AutoSteps.Add(new int[5] { 100, 91, 50, 10, 5 });
                    c.AutoSteps.Add(new int[5] { 110, 101, 60, 10, 5 });
                    c.AutoSteps.Add(new int[5] { 120, 111, 70, 10, 5 });
                }
                if (c.UpAutoSteps.Count == 0)
                {
                    c.UpAutoSteps.Add(new int[3] { 50, 41, 10 });
                    c.UpAutoSteps.Add(new int[3] { 60, 51, 20 });
                    c.UpAutoSteps.Add(new int[3] { 70, 61, 30 });
                    c.UpAutoSteps.Add(new int[3] { 80, 71, 40 });
                }
                if (c.DownAutoSteps.Count == 0)
                {
                    c.DownAutoSteps.Add(new int[3] { 50, 41, 10 });
                    c.DownAutoSteps.Add(new int[3] { 60, 51, 20 });
                    c.DownAutoSteps.Add(new int[3] { 70, 61, 30 });
                    c.DownAutoSteps.Add(new int[3] { 80, 71, 40 });
                }

                return c;
            }
            catch (Exception)
            {
                return CreateDefaultConfig();
            }
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
            conf.WriteConfig();
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
