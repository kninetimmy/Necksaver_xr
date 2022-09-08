using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHM_Monitor
{
    public partial class SHMMonForm : Form
    {
        MemoryMappedFile shm;
        MemoryMappedViewAccessor accessor;
        MemoryMappedFile shmN;
        MemoryMappedViewAccessor accessorN;
        MemoryMappedFile shmV;
        MemoryMappedViewAccessor accessorV;
        public shmVal_s shmValues;
        public char[,] debNames;
        public char[,] debValues;

        public struct shmVal_s
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
        public List<string> varNames = new List<string>()
        {
        "hmdYawAngle",
        "hmdPitchAngle",
        "yawOffset",
        "pitchOffset",
        "lateralOffset",
        "longitudinalOffset",
        "rightMultiplier",
        "leftMultiplier",
        "upMultiplier",
        "downMultiplier",
        "leftStartAt",
        "rightStartAt",
        "upStartAt",
        "downStartAt",
        "resetHmdOrientation",
        "useLinearRotation",
        "useLinearPitchRotation",
        "holdLinearRotation",
        "holdLinearPitchRotation",
        "hasBeenCentered"
    };

        public SHMMonForm()
        {
            InitializeComponent();
            string shmName = "XRNeckSaferSHM";
            int shmSize = 80;

            shm = MemoryMappedFile.CreateOrOpen(shmName, shmSize);
            accessor = shm.CreateViewAccessor();

        }

        public void generateOutput()
        {
            accessor.Read<shmVal_s>(0, out shmValues);

            StringBuilder sb = new StringBuilder();

            sb.Append(generateLine("hmdYawAngle" , shmValues.hmdYawAngle.ToString()));
            sb.Append(generateLine("hmdPitchAngle" , shmValues.hmdPitchAngle.ToString()));
            sb.Append(generateLine("yawOffset" , shmValues.yawOffset.ToString()));
            sb.Append(generateLine("pitchOffset" , shmValues.pitchOffset.ToString()));
            sb.Append(generateLine("lateralOffset" , shmValues.lateralOffset.ToString()));
            sb.Append(generateLine("longitudinalOffset" , shmValues.longitudinalOffset.ToString()));
            sb.Append(generateLine("rightMultiplier" , shmValues.rightMultiplier.ToString()));
            sb.Append(generateLine("leftMultiplier" , shmValues.leftMultiplier.ToString()));
            sb.Append(generateLine("upMultiplier" , shmValues.upMultiplier.ToString()));
            sb.Append(generateLine("downMultiplier" , shmValues.downMultiplier.ToString()));
            sb.Append(generateLine("leftStartAt" , shmValues.leftStartAt.ToString()));
            sb.Append(generateLine("rightStartAt" , shmValues.rightStartAt.ToString()));
            sb.Append(generateLine("upStartAt" , shmValues.upStartAt.ToString()));
            sb.Append(generateLine("downStartAt" , shmValues.downStartAt.ToString()));
            sb.Append(generateLine("resetHmdOrientation" , shmValues.resetHmdOrientation.ToString()));
            sb.Append(generateLine("useLinearRotation" , shmValues.useLinearRotation.ToString()));
            sb.Append(generateLine("useLinearPitchRotation" , shmValues.useLinearPitchRotation.ToString()));
            sb.Append(generateLine("holdLinearRotation" , shmValues.holdLinearRotation.ToString()));
            sb.Append(generateLine("holdLinearPitchRotation" , shmValues.holdLinearPitchRotation.ToString()));
            sb.Append(generateLine("hasBeenCentered" , shmValues.hasBeenCentered.ToString()));

            OutputTextBox.Text = sb.ToString();
        }

        public string generateLine(string s, string v)
        {
            string line = String.Format("{0,-30} {1,-20}", s, v)+ Environment.NewLine;
            return line;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            generateOutput();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            generateOutput();
        }

        private void AutoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoCheckBox.Checked)
            {
                timer1.Interval = (int)updateInterval.Value;
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }

        }

        private void updateInterval_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)updateInterval.Value;
        }
    }
}
