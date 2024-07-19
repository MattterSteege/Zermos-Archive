using System;
using System.Collections.Generic;
using System.Globalization;
using Zermos_Web.Models.SomtodayGradesModel;

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
            var dotIndex = input.IndexOf('.');
            if (dotIndex != -1)
            {
                input = input.Remove(dotIndex, 1);
                input = input.Insert(dotIndex, ".");
            }

            // Try to parse the input as a float
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out result)) return true;

            result = 0f;
            return false;
        }

        public static float ParseFloat(string input)
        {
            if (TryParseFloat(input, out var result)) return result;
            return 10f;
            throw new FormatException("Input string was not in a correct format.");
        }

        public static IList<double?> CalculateWeightedAverageSnapshots(float[] grades, int[] weights)
        {
            if (grades.Length != weights.Length)
                throw new ArgumentException("The number of grades must match the number of weights.");

            IList<double?> snapshots = new List<double?>();

            double sum = 0;
            double weightSum = 0;

            for (var i = 0; i < grades.Length; i++)
            {
                if (weights[i] == 0)
                    continue;
                
                
                sum += grades[i] * weights[i];
                weightSum += weights[i];

                var currentAverage = sum / weightSum;
                snapshots.Add(currentAverage);
            }

            return snapshots;
        }
        
        public static List<double> CalculateWeightedAverageSnapshots(double[] cijfers, int[] weights)
        {
            if (cijfers.Length != weights.Length)
                throw new ArgumentException("The number of grades must match the number of weights.");

            List<double> snapshots = new List<double>();

            double sum = 0;
            double weightSum = 0;
            
            for (var i = 0; i < cijfers.Length; i++)
            {
                if (weights[i] == 0)
                    continue;
                
                sum += cijfers[i] * weights[i];
                weightSum += weights[i];

                var currentAverage = sum / weightSum;
                snapshots.Add(currentAverage);
            }
            
            return snapshots;
        }
            

        public static IList<double?> CalculateStandardDeviationSnapshots(float[] grades, int[] weights)
        {
            if (grades.Length != weights.Length)
                throw new ArgumentException("The number of grades must match the number of weights.");

            IList<double?> snapshots = new List<double?>();

            double sum = 0;
            double weightSum = 0;

            for (var i = 0; i < grades.Length; i++)
            {
                sum += grades[i] * weights[i];
                weightSum += weights[i];

                var currentAverage = sum / weightSum;
                snapshots.Add(currentAverage);
            }

            return snapshots;
        }

        public static double RoundApproximate(float dbl, int digits, double margin, MidpointRounding mode)
        {
            var fraction = dbl * Math.Pow(10, digits);
            var value = Math.Truncate(fraction);
            fraction = fraction - value;
            if (fraction == 0)
                return dbl;

            var tolerance = margin * dbl;
            // Determine whether this is a midpoint value.
            if ((fraction >= .5 - tolerance) & (fraction <= .5 + tolerance))
            {
                if (mode == MidpointRounding.AwayFromZero)
                    return (value + 1) / Math.Pow(10, digits);
                if (value % 2 != 0)
                    return (value + 1) / Math.Pow(10, digits);
                return value / Math.Pow(10, digits);
            }

            // Any remaining fractional value greater than .5 is not a midpoint value.
            if (fraction > .5)
                return (value + 1) / Math.Pow(10, digits);
            return value / Math.Pow(10, digits);
        }
        
        public static double RoundApproximate(this float dbl, int digits)
        {
            return RoundApproximate(dbl, digits, 0.0001, MidpointRounding.AwayFromZero);
        }
        
        public static float ToFloat(this string input)
        {
            if (input == "O") return 5;
            if (input == "V") return 7;
            if (input == "G") return 8;
            
            return ParseFloat(input);
        }

        /// <summary>
        /// Rounds the grade using the setting of the user
        /// </summary>
        /// <param name="grade">The grade to round</param>
        /// <a href="https://www.onlineslagen.nl/hoe-moeten-cijfers-worden-afgerond">Online berekening uitleg</a>
        public static float ExamenAfronding(this float grade)
        {
            if (grade < 0 || grade > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(grade), "Grade must be between 0 and 10");
            }

            if (grade >= 9.5f)
            {
                return 10f;
            }

            return (float)Math.Floor(grade + 0.5f);
        }
    }
}