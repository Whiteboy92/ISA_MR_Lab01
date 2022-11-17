using System;
using System.Globalization;
using Eto.Forms;


namespace ISA_Ryba_Marcin
{
    public static class FormatChecker
    {
        public static bool ParseDouble(string text, string fieldName, out double output, string culture = "")
        {
            if (culture.Length == 0)
            {
                culture = CultureInfo.CurrentCulture.ToString();
            }

            try
            {
                output = double.Parse(text, NumberStyles.Number, new CultureInfo(culture));
                return true;
            }

            catch (ArgumentNullException)
            {
                MessageBox.Show($"{fieldName} is empty", MessageBoxType.Error);
            }
            
            catch (FormatException)
            {
                MessageBox.Show($"{fieldName} formatting isn't correct, try -> {5.1}", MessageBoxType.Error);
            }
            
            catch (OverflowException)
            {
                MessageBox.Show($"{fieldName} Value should start from {double.MinValue} to {double.MaxValue}", MessageBoxType.Error);
            }

            output = double.NaN;
            return false;
        }
        
        public static bool ParseLong(string text, string fieldName, out long output, string culture = "")
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

            catch (ArgumentNullException)
            {
                MessageBox.Show($"{fieldName} is empty", MessageBoxType.Error);
            }
            
            catch (ArgumentException)
            {
                MessageBox.Show($"{fieldName} formatting isn't correct, try -> {5}", MessageBoxType.Error);
            }
            
            catch (FormatException)
            {
                MessageBox.Show($"{fieldName} formatting isn't correct, try -> {5}", MessageBoxType.Error);
            }
            
            catch (OverflowException)
            {
                MessageBox.Show($"{fieldName} Value should start from {long.MinValue} to {long.MaxValue}", MessageBoxType.Error);
            }

            output = 0;
            return false;
        }
    }
}