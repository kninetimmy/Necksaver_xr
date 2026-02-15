using System.Collections.Generic;
using SharpDX;

namespace XRNeckSafer
{
    public static class RotationCalculator
    {
        public static void CalcAutoRotAndTrans(int yaw, List<int[]> steps, ref int arot, ref Vector3 atrans)
        {
            if (steps == null || steps.Count == 0)
            {
                return;
            }

            int yawsign = yaw > 0 ? 1 : -1;
            int absyaw = yaw * yawsign;
            int absarot = arot > 0 ? arot : -arot;
            int autorot = 0;
            int transx = 0;
            int transz = 0;

            int deact = 0;

            for (int i = 0; i < steps.Count; i++)
            {
                int[] step = steps[i];
                if (step == null || step.Length < 5)
                {
                    continue;
                }

                int act = step[0];
                deact = step[1];
                int rot = step[2];
                int tx = step[3];
                int tz = step[4];

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

            if (absarot > autorot && absyaw >= deact)
            {
                return;
            }

            arot = yawsign * autorot;
            atrans.X = (float)transx / 100.0F * -yawsign;
            atrans.Z = (float)transz / 100.0F;
        }

        public static void CalcAutoPitch(int pitch, List<int[]> steps, ref int arot)
        {
            if (steps == null || steps.Count == 0)
            {
                return;
            }

            int pitchsign = pitch > 0 ? 1 : -1;
            int abspitch = pitch > 0 ? pitch : -pitch;
            int autorot = 0;
            int absarot = arot > 0 ? arot : -arot;

            int deact = 0;

            for (int i = 0; i < steps.Count; i++)
            {
                int[] step = steps[i];
                if (step == null || step.Length < 3)
                {
                    continue;
                }

                int act = step[0];
                deact = step[1];
                int rot = step[2];

                if (abspitch >= act)
                {
                    autorot = rot;
                }
                else
                {
                    break;
                }
            }

            if (absarot > autorot && abspitch >= deact)
            {
                return;
            }

            arot = autorot * pitchsign;
        }
    }
}
