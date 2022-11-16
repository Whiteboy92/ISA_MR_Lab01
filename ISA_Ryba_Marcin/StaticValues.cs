using System;

namespace ISA_Ryba_Marcin
{
    public static class StaticValues
    {
        public static double Pk = 0.5;
        public static double Pm = 0.005;
        
        public static double A;
        public static double B;
        public static double D;
        
        public static int L;

        public static readonly Random Rand = new();
        
        public static TargetFunction TargetFunction = TargetFunction.Max;

        public static double RandomXReal()
        {
            var accuracy = MathHelper.Accuracy(D);
            var trueXReal = Rand.NextDouble() * (B - A) + A;
            return Math.Round(trueXReal, accuracy);
        }
    }
}