using SharpDX;
using System.Collections.Generic;

namespace XRNeckSafer
{
    public static class RotationCalculator
    {
        public static void CalcAutoRotAndTrans(int yaw, List<int[]> steps, ref int arot, ref Vector3 atrans)
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

            for (int i = 0; i < steps.Count; i++)
            {
                act = steps[i][0];
                deact = steps[i][1];
                rot = steps[i][2];
                tx = steps[i][3];
                tz = steps[i][4];

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

        public static void CalcAutoPitch(int pitch, List<int[]> steps, ref int arot)
        {
            int pitchsign = (pitch > 0) ? 1 : -1;
            int abspitch = (pitch > 0) ? pitch : -pitch;
            int autorot = 0;
            int absarot = (arot > 0) ? arot : -arot;

            int act;
            int deact = 0;
            int rot;

            for (int i = 0; i < steps.Count; i++)
            {
                act = steps[i][0];
                deact = steps[i][1];
                rot = steps[i][2];

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
    }
}
