using System;
using System.Globalization;

namespace Zermos_Web.Utilities
{
    public static class NumberUtils
    {
        public static bool TryParseFloat(string input, out float result)
        {
            // Check if the input is null or empty
            if (string.IsNullOrEmpty(input))
            {
                result = 0f;
                return false;
            }

            // Replace commas with dots, and remove any additional dots
            input = input.Replace(",", ".");
            int dotIndex = input.IndexOf('.');
            if (dotIndex != -1)
            {
                input = input.Remove(dotIndex, 1);
                input = input.Insert(dotIndex, ".");
            }

            // Try to parse the input as a float
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return true;
            }
            else
            {
                result = 0f;
                return false;
            }
        }
        
        public static float ParseFloat(string input) 
        {
            if(TryParseFloat(input, out var result)){
                return result;
            }
            throw new FormatException("Input string was not in a correct format.");
        }
        
        public static double RoundApproximate(double dbl, int digits, double margin, MidpointRounding mode)
        {
            double fraction = dbl * Math.Pow(10, digits);
            double value = Math.Truncate(fraction);
            fraction = fraction - value;
            if (fraction == 0)
                return dbl;

            double tolerance = margin * dbl;
            // Determine whether this is a midpoint value.
            if ((fraction >= .5 - tolerance) & (fraction <= .5 + tolerance))
            {
                if (mode == MidpointRounding.AwayFromZero)
                    return (value + 1) / Math.Pow(10, digits);
                else
                if (value % 2 != 0)
                    return (value + 1) / Math.Pow(10, digits);
                else
                    return value / Math.Pow(10, digits);
            }
            // Any remaining fractional value greater than .5 is not a midpoint value.
            if (fraction > .5)
                return (value + 1) / Math.Pow(10, digits);
            else
                return value / Math.Pow(10, digits);
        }
    }
}