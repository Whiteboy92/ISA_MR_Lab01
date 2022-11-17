using System;

namespace ISA_Ryba_Marcin
{
    public static class MathHelper
    {
        public static int Accuracy(double d)
        {
            return d switch
            {
                1.0 => 0,
                0.1 => 1,
                0.01 => 2,
                0.001 => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };
        }

        
        
        public static long XBinToXInt(string xBin)
        {
            return Convert.ToInt64(xBin, 2);
        }

        public static double XIntToXReal(long xInt)
        {
            double trueXReal = ((StaticValues.B - StaticValues.A) * xInt) / (Math.Pow(2.0, StaticValues.L) - 1.0) + StaticValues.A;
            return Math.Round(trueXReal, Accuracy(StaticValues.D));
        }

        public static double XBinToXReal(string xBin)
        {
            return XIntToXReal(XBinToXInt(xBin));
        }

        public static double Fx(double xReal)
        {
            return (xReal % 1.0) * (Math.Cos(20.0 * Math.PI * xReal) - Math.Sin(xReal));
        }

        public static long XRealToXInt(double xReal)
        {
            return (long)Math.Round((1.0 / (StaticValues.B - StaticValues.A)) * (xReal - StaticValues.A) * ((Math.Pow(2.0, StaticValues.L)) - 1.0));
        }

        public static string XIntToXBin(long xInt)
        {
            return Convert.ToString(xInt, 2).PadLeft(StaticValues.L, '0');
        }
    }
}