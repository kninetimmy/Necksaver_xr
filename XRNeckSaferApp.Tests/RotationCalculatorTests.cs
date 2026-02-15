using System.Collections.Generic;
using SharpDX;
using Xunit;

namespace XRNeckSafer.Tests
{
    public class RotationCalculatorTests
    {
        [Fact]
        public void CalcAutoRotAndTrans_UsesBoundaryStepWhenYawEqualsActivation()
        {
            var steps = new List<int[]>
            {
                new[] { 10, 5, 15, 20, 30 },
                new[] { 20, 10, 25, 40, 60 }
            };

            int arot = 0;
            var trans = new Vector3(0, 2, 0);

            RotationCalculator.CalcAutoRotAndTrans(10, steps, ref arot, ref trans);

            Assert.Equal(15, arot);
            Assert.Equal(-0.2f, trans.X, 3);
            Assert.Equal(0.3f, trans.Z, 3);
            Assert.Equal(2f, trans.Y);
        }

        [Fact]
        public void CalcAutoRotAndTrans_LeavesOutputsUnchangedWhenStepsNullOrEmpty()
        {
            int arot = 7;
            var trans = new Vector3(1, 2, 3);

            RotationCalculator.CalcAutoRotAndTrans(30, null, ref arot, ref trans);
            Assert.Equal(7, arot);
            Assert.Equal(new Vector3(1, 2, 3), trans);

            RotationCalculator.CalcAutoRotAndTrans(30, new List<int[]>(), ref arot, ref trans);
            Assert.Equal(7, arot);
            Assert.Equal(new Vector3(1, 2, 3), trans);
        }

        [Fact]
        public void CalcAutoRotAndTrans_SkipsMalformedRowsWithoutThrowing()
        {
            var steps = new List<int[]>
            {
                null,
                new[] { 5, 2, 10 },
                new[] { 10, 5, 20, 30, 40 }
            };

            int arot = 0;
            var trans = new Vector3();

            var ex = Record.Exception(() => RotationCalculator.CalcAutoRotAndTrans(12, steps, ref arot, ref trans));

            Assert.Null(ex);
            Assert.Equal(20, arot);
            Assert.Equal(-0.3f, trans.X, 3);
            Assert.Equal(0.4f, trans.Z, 3);
        }


        [Fact]
        public void CalcAutoRotAndTrans_UsesZeroSignWhenYawIsZero()
        {
            var steps = new List<int[]>
            {
                new[] { 0, 0, 15, 20, 30 }
            };

            int arot = 5;
            var trans = new Vector3(1, 2, 3);

            RotationCalculator.CalcAutoRotAndTrans(0, steps, ref arot, ref trans);

            Assert.Equal(0, arot);
            Assert.Equal(0f, trans.X);
            Assert.Equal(0.3f, trans.Z, 3);
            Assert.Equal(2f, trans.Y);
        }


        [Fact]
        public void CalcAutoRotAndTrans_LeavesOutputUnchangedWhenAllRowsMalformed()
        {
            var steps = new List<int[]>
            {
                null,
                new[] { 1, 1, 1 },
                new[] { 2, 2, 2, 2 }
            };

            int arot = 4;
            var trans = new Vector3(1, 2, 3);

            RotationCalculator.CalcAutoRotAndTrans(12, steps, ref arot, ref trans);

            Assert.Equal(4, arot);
            Assert.Equal(new Vector3(1, 2, 3), trans);
        }

        [Fact]
        public void CalcAutoPitch_UsesBoundaryStepWhenPitchEqualsActivation()
        {
            var steps = new List<int[]>
            {
                new[] { 7, 3, 11 },
                new[] { 14, 7, 22 }
            };

            int arot = 0;

            RotationCalculator.CalcAutoPitch(7, steps, ref arot);

            Assert.Equal(11, arot);
        }

        [Fact]
        public void CalcAutoPitch_LeavesOutputUnchangedWhenStepsNullOrEmpty()
        {
            int arot = -9;

            RotationCalculator.CalcAutoPitch(18, null, ref arot);
            Assert.Equal(-9, arot);

            RotationCalculator.CalcAutoPitch(18, new List<int[]>(), ref arot);
            Assert.Equal(-9, arot);
        }


        [Fact]
        public void CalcAutoPitch_UsesZeroSignWhenPitchIsZero()
        {
            var steps = new List<int[]>
            {
                new[] { 0, 0, 11 }
            };

            int arot = -9;

            RotationCalculator.CalcAutoPitch(0, steps, ref arot);

            Assert.Equal(0, arot);
        }

        [Fact]
        public void CalcAutoPitch_LeavesOutputUnchangedWhenAllRowsMalformed()
        {
            var steps = new List<int[]>
            {
                null,
                new[] { 1 },
                new[] { 2, 1 }
            };

            int arot = 6;

            RotationCalculator.CalcAutoPitch(12, steps, ref arot);

            Assert.Equal(6, arot);
        }

        [Fact]
        public void CalcAutoPitch_SkipsMalformedRowsWithoutThrowing()
        {
            var steps = new List<int[]>
            {
                null,
                new[] { 3 },
                new[] { 6, 2, 13 }
            };

            int arot = 0;

            var ex = Record.Exception(() => RotationCalculator.CalcAutoPitch(6, steps, ref arot));

            Assert.Null(ex);
            Assert.Equal(13, arot);
        }
    }
}
