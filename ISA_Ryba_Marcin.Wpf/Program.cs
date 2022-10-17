using System;
using Eto.Forms;

namespace ISA_Ryba_Marcin.Wpf
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Wpf).Run(new MainForm());
        }
    }
}