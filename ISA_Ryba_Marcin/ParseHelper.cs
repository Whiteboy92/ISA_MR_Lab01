using System;
using System.Globalization;
using Eto.Forms;
using Gtk;
using static System.Double;

namespace ISA_Ryba_Marcin
{
    public class ParseHelper
    {
        public static bool ParseDouble(string text, string fieldName, out double output, string culture = "")
        {
            if (culture.Length == 0)
            {
                culture = CultureInfo.CurrentCulture.ToString();
            }

            try
            {
                output = Parse(text, NumberStyles.Number, new CultureInfo(culture));
                return true;
            }

            catch (ArgumentNullException _)
            {
                MessageBox.Show($"{fieldName} jest puste", MessageBoxType.Error);
            }
            
            catch (FormatException _)
            {
                MessageBox.Show($"{fieldName} nie posiada poprawnego formatu - {5.1}", MessageBoxType.Error);
            }
            
            catch (OverflowException _)
            {
                MessageBox.Show($"{fieldName} przedział powinien być od {MinValue} do {MaxValue}", MessageBoxType.Error);
            }

            output = double.NaN;
            return false;
        }
        
        public static bool ParseLong(string text, string fieldName, out double output, string culture = "")
        {
            if (culture.Length == 0)
            {
                culture = CultureInfo.CurrentCulture.ToString();
            }

            try
            {
                output = long.Parse(text, NumberStyles.Integer, new CultureInfo(culture));
                return true;
            }

            catch (ArgumentNullException _)
            {
                MessageBox.Show($"{fieldName} jest puste", MessageBoxType.Error);
            }
            
            catch (ArgumentException _)
            {
                MessageBox.Show($"{fieldName} nie posiada poprawnego formatu - {5}", MessageBoxType.Error);
            }
            
            catch (FormatException _)
            {
                MessageBox.Show($"{fieldName} nie posiada poprawnego formatu - {5}", MessageBoxType.Error);
            }
            
            catch (OverflowException _)
            {
                MessageBox.Show($"{fieldName} przedział powinien być od {long.MinValue} do {long.MaxValue}", MessageBoxType.Error);
            }

            output = double.NaN;
            return false;
        }
    }
}