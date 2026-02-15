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

            int yawsign = SignOrZero(yaw);
            int absyaw = System.Math.Abs(yaw);
            int absarot = System.Math.Abs(arot);
            int autorot = 0;
            int transx = 0;
            int transz = 0;
            int deact = 0;
            bool hasValidStep = false;

            for (int i = 0; i < steps.Count; i++)
            {
                int[] step = steps[i];
                if (step == null || step.Length < 5)
                {
                    continue;
                }

                hasValidStep = true;

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

            if (!hasValidStep)
            {
                return;
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

            int pitchsign = SignOrZero(pitch);
            int abspitch = System.Math.Abs(pitch);
            int autorot = 0;
            int absarot = System.Math.Abs(arot);
            int deact = 0;
            bool hasValidStep = false;

            for (int i = 0; i < steps.Count; i++)
            {
                int[] step = steps[i];
                if (step == null || step.Length < 3)
                {
                    continue;
                }

                hasValidStep = true;

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

            if (!hasValidStep)
            {
                return;
            }

            if (absarot > autorot && abspitch >= deact)
            {
                return;
            }

            arot = autorot * pitchsign;
        }

        private static int SignOrZero(int value)
        {
            if (value > 0)
            {
                return 1;
            }

            if (value < 0)
            {
                return -1;
            }

            return 0;
        }
    }
}
