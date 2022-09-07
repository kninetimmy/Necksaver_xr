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
        public shmVal_s shmValues;

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

            sb.Append("hmdYawAngle" + shmValues.hmdYawAngle + Environment.NewLine);
            sb.Append("hmdPitchAngle" + shmValues.hmdPitchAngle + Environment.NewLine);
            sb.Append("yawOffset" + shmValues.yawOffset + Environment.NewLine);
            sb.Append("pitchOffset" + shmValues.pitchOffset + Environment.NewLine);
            sb.Append("lateralOffset" + shmValues.lateralOffset + Environment.NewLine);
            sb.Append("longitudinalOffset" + shmValues.longitudinalOffset + Environment.NewLine);
            sb.Append("rightMultiplier" + shmValues.rightMultiplier + Environment.NewLine);
            sb.Append("leftMultiplier" + shmValues.leftMultiplier + Environment.NewLine);
            sb.Append("upMultiplier" + shmValues.upMultiplier + Environment.NewLine);
            sb.Append("downMultiplier" + shmValues.downMultiplier + Environment.NewLine);
            sb.Append("leftStartAt" + shmValues.leftStartAt + Environment.NewLine);
            sb.Append("rightStartAt" + shmValues.rightStartAt + Environment.NewLine);
            sb.Append("upStartAt" + shmValues.upStartAt + Environment.NewLine);
            sb.Append("downStartAt" + shmValues.downStartAt + Environment.NewLine);
            sb.Append("resetHmdOrientation" + shmValues.resetHmdOrientation + Environment.NewLine);
            sb.Append("useLinearRotation" + shmValues.useLinearRotation + Environment.NewLine);
            sb.Append("useLinearPitchRotation" + shmValues.useLinearPitchRotation + Environment.NewLine);
            sb.Append("holdLinearRotation" + shmValues.holdLinearRotation + Environment.NewLine);
            sb.Append("holdLinearPitchRotation" + shmValues.holdLinearPitchRotation + Environment.NewLine);
            sb.Append("hasBeenCentered" + shmValues.hasBeenCentered + Environment.NewLine);
        }

        public string generateLine(string s, string v)
        {
            string line = "";

            return line;
        }
    }
}
