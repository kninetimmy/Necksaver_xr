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
        public ButtonConfig copyButtonConfig(ButtonConfig b)
        {
            b.JoystickGUID = string.Copy(JoystickGUID);
            b.Button = string.Copy(Button);
            b.ModJoystickGUID = string.Copy(ModJoystickGUID);
            b.ModButton = string.Copy(ModButton);
            b.UseModifier = UseModifier;
            b.Use8WayHat = Use8WayHat;
            b.Invert = Invert;
            b.Toggle = Toggle;

            return b;
        }

    }

    public class Config
    {
        public ButtonConfig LeftButton;
        public ButtonConfig LeftButton2;
        public ButtonConfig LeftButton3;
        public ButtonConfig RightButton;
        public ButtonConfig RightButton2;
        public ButtonConfig RightButton3;
        public ButtonConfig ResetButton;
        public ButtonConfig ResetButton2;
        public ButtonConfig ResetButton3;
        public ButtonConfig HoldButton1;
        public ButtonConfig HoldButton2;
        public ButtonConfig HoldButton3;
        public ButtonConfig HoldButton4;
        public ButtonConfig AccuResetButton;
        public ButtonConfig AccuResetButton2;
        public ButtonConfig AccuResetButton3;
        public int Angle;
        public int TransLR;
        public int TransF;
        public bool Additiv;
        public bool Auto;
        public bool StartMinimized;
        public bool MinimizeToTray;
        public bool MultipleLRbuttons;
        public int PitchLimForAutorot;
        public static string configfilename;
        public List<int[]> AutoSteps;


        public Config()
        {
            LeftButton = new ButtonConfig();
            LeftButton2 = new ButtonConfig();
            LeftButton3 = new ButtonConfig();
            RightButton = new ButtonConfig();
            RightButton2 = new ButtonConfig();
            RightButton3 = new ButtonConfig();
            ResetButton = new ButtonConfig();
            ResetButton2 = new ButtonConfig();
            ResetButton3 = new ButtonConfig();
            HoldButton1 = new ButtonConfig();
            HoldButton2 = new ButtonConfig();
            HoldButton3 = new ButtonConfig();
            HoldButton4 = new ButtonConfig();
            AccuResetButton = new ButtonConfig();
            AccuResetButton2 = new ButtonConfig();
            AccuResetButton3 = new ButtonConfig();
            Angle = 30;
            TransLR = 0;
            TransF = 0;
            Additiv = false;
            Auto = false;
            StartMinimized = false;
            MinimizeToTray = false;
            MultipleLRbuttons = false;
            PitchLimForAutorot = 90;
            AutoSteps = new List<int[]>();
        }

        static public Config ReadConfig()
        {
            try
            {
                configfilename = @".\XRNeckSafer.cfg";
                string[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                    configfilename = @".\" + args[1];

                Config c = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configfilename));
                if (c.LeftButton   == null) c.LeftButton    = new ButtonConfig();
                if (c.LeftButton2  == null) c.LeftButton2   = new ButtonConfig();
                if (c.LeftButton3  == null) c.LeftButton3   = new ButtonConfig();
                if (c.RightButton  == null) c.RightButton   = new ButtonConfig();
                if (c.RightButton2 == null) c.RightButton2  = new ButtonConfig();
                if (c.RightButton3 == null) c.RightButton3  = new ButtonConfig();
                if (c.ResetButton  == null) c.ResetButton   = new ButtonConfig();
                if (c.ResetButton2 == null) c.ResetButton2  = new ButtonConfig();
                if (c.ResetButton3 == null) c.ResetButton3  = new ButtonConfig();
                if (c.HoldButton1  == null) c.HoldButton1   = new ButtonConfig();
                if (c.HoldButton2  == null) c.HoldButton2   = new ButtonConfig();
                if (c.HoldButton3  == null) c.HoldButton3   = new ButtonConfig();
                if (c.HoldButton4  == null) c.HoldButton4  =  new ButtonConfig();
                if (c.AccuResetButton == null) c.AccuResetButton = new ButtonConfig();
                if (c.AccuResetButton2 == null) c.AccuResetButton2 = new ButtonConfig();
                if (c.AccuResetButton3 == null) c.AccuResetButton3 = new ButtonConfig();

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
                return c;
            }
            catch (Exception)
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

                conf.WriteConfig();
                return conf;
            }
        }

        public void WriteConfig()
        {
            File.WriteAllText(configfilename, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
