using System;
using UnityEngine;

namespace RoadTurtleGames.StarUtilities
{
    public static class StarUtilities
    {
        /// <summary>
        /// The minimum temperature in Kelvin used for normalization.
        /// </summary>
        public const int TemperatureMin = 2500;
        /// <summary>
        /// The maximum temperature in Kelvin used for normalization.
        /// </summary>
        public const int TemperatureMax = 40000;

        /// <summary>
        /// The minimum exponent of luminosity used for normalization.
        /// </summary>
        public const int LuminosityExponentMin = -5;
        /// <summary>
        /// The maximum exponent of luminosity used for normalization.
        /// </summary>
        public const int LuminosityExponentMax = 5;

        /// <summary>
        /// The temperature of the Sun in Kelvin.
        /// </summary>
        public const int SunTemperature = 5778;
        /// <summary>
        /// The radius of the Sun in kilometers.
        /// </summary>
        public const int SunRadius = 696340;
        /// <summary>
        /// The luminosity of the Sun.
        /// </summary>
        public const double SunLuminosity = 3.84e26;

        public const double StefanBoltzmannConstant = 0.0000000567;

        #region Classification

        /// <summary>
        /// Returns the Spectral Class/Type of a star based on it's temperature in Kelvin.
        /// https://en.wikipedia.org/wiki/Stellar_classification
        /// </summary>
        /// <param name="temperature">The temperature in Kelvin</param>
        /// <returns>The string Class/Type of the star.</returns>
        public static string GetSpectralClass(int temperature)
        {
            if (temperature < 2400)
                return "?";

            if (MathExtensions.IsInRange(temperature, 2400, 3700))
                return "M" + GetSubdivision(temperature, 2400, 3700);
            else if (MathExtensions.IsInRange(temperature, 3700, 5200))
                return "K" + GetSubdivision(temperature, 3700, 5200);
            else if (MathExtensions.IsInRange(temperature, 5200, 6000))
                return "G" + GetSubdivision(temperature, 5200, 6000);
            else if (MathExtensions.IsInRange(temperature, 6000, 7500))
                return "F" + GetSubdivision(temperature, 6000, 7500);
            else if (MathExtensions.IsInRange(temperature, 7500, 10000))
                return "A" + GetSubdivision(temperature, 7500, 10000);
            else if (MathExtensions.IsInRange(temperature, 10000, 30000))
                return "B" + GetSubdivision(temperature, 10000, 30000);
            else
                return "O";
        }

        /// <summary>
        /// Returns a 0-9 subdivision of a value based on the given min and max.
        /// https://en.wikipedia.org/wiki/Stellar_classification
        /// </summary>
        /// <param name="v">The input value.</param>
        /// <param name="min">The minimum of the range.</param>
        /// <param name="max">The maximum of the range.</param>
        /// <returns>A 0-9 value, where 0 is largest and 9 is smallest.</returns>
        public static string GetSubdivision(int v, int min, int max)
        {
            return ((int)((1 - Mathf.InverseLerp(min, max, v)) * 10)).ToString();
        }

        #endregion

        #region Luminosity

        /// <summary>
        /// Estimates the Luminosity of a star based on its temperature and radius.
        /// https://en.wikipedia.org/wiki/Stefan%E2%80%93Boltzmann_law#Examples
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin.</param>
        /// <param name="radius">Radius in kilometers.</param>
        /// <returns>The estimated luminosity.</returns>
        public static double GetEstimatedLuminosity(float temperature, float radius)
        {
            double radiusMeters = radius * 1000;

            double radiusSquared = radiusMeters * radiusMeters;
            double temperatureFourth = temperature * temperature * temperature * temperature;

            return 4 * Math.PI * radiusSquared * StefanBoltzmannConstant * temperatureFourth;
        }

        /// <summary>
        /// Estimates the Solar Luminosity (Luminosity relative to the Sun) of a star based on its temperature and radius.
        /// https://en.wikipedia.org/wiki/Stefan%E2%80%93Boltzmann_law#Examples
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin.</param>
        /// <param name="radius">Radius in kilometers.</param>
        /// <returns>The estimated Solar Luminosity.</returns>
        public static float GetEstimatedSolarLuminosity(float temperature, float radius)
        {
            double estimatedLuminosity = GetEstimatedLuminosity(temperature, radius);
            return (float)(estimatedLuminosity / SunLuminosity);
        }

        /// <summary>
        /// Estimates the Solar Luminosity (Luminosity relative to the Sun) exponent (base 10) of a star based on its temperature and radius.
        /// https://en.wikipedia.org/wiki/Stefan%E2%80%93Boltzmann_law#Examples
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin.</param>
        /// <param name="radius">Radius in kilometers.</param>
        /// <returns>The estimated exponent of Solar Luminosity (base 10).</returns>
        public static float GetEstimatedSolarLuminosityExponent(float temperature, float radius)
        {
            double estimatedLuminosity = GetEstimatedLuminosity(temperature, radius);
            double relativeLuminosity = estimatedLuminosity / SunLuminosity;

            float exponent = (float)Math.Log10(relativeLuminosity);
            return exponent;
        }

        /// <summary>
        /// Gets the normalized Solar Luminsoity exponent estimated from the temperature and radius, normalized based on LuminosityExponentMin and LuminosityExponentMax.
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin.</param>
        /// <param name="radius">Radius in Kilometers.</param>
        /// <returns>The normalized luminosity.</returns>
        public static float GetNormalizedEstimatedSolarLuminosityExponent(float temperature, float radius)
        {
            float exponent = GetEstimatedSolarLuminosityExponent(temperature, radius);
            return Mathf.InverseLerp(LuminosityExponentMin, LuminosityExponentMax, exponent);
        }

        /// <summary>
        /// Gets the real luminosity value from a normalized luminosity exponent.
        /// https://en.wikipedia.org/wiki/Stefan%E2%80%93Boltzmann_law#Examples
        /// </summary>
        /// <param name="normalizedLuminosity">Normalized luminosity exponent based on LuminosityExponentMin and LuminosityExponentMax.</param>
        /// <returns>The real luminosity.</returns>
        public static double GetLuminosity(float normalizedLuminosity)
        {
            float exponent = Mathf.Lerp(LuminosityExponentMin, LuminosityExponentMax, normalizedLuminosity);
            float solarLuminosity = Mathf.Pow(10, exponent);

            return SunLuminosity * solarLuminosity;
        }

        #endregion

        #region Temperature

        /// <summary>
        /// Gets the normalized temperature based on TemperatureMin and TemperatureMax.
        /// Note that the scale is logarithmic.
        /// </summary>
        /// <param name="temperature">The temperature in Kelvin.</param>
        /// <returns>The normalized temperature.</returns>
        public static float GetNormalizedTemperature(float temperature)
        {
            float b = (float)temperature / TemperatureMin;
            if (b == 1)
                return 0;

            float l = Mathf.Log(TemperatureMax / TemperatureMin, b);
            return 1 / l;
        }

        /// <summary>
        /// Gets the real temperature based on a normalized value.
        /// Note that the scale is logarithmic.
        /// </summary>
        /// <param name="normalizedTemperature">The normalized temperature relative to TemperatureMin and TemperatureMax.</param>
        /// <returns>The real temperature.</returns>
        public static float GetTemperature(float normalizedTemperature)
        {
            float b = TemperatureMax / TemperatureMin;
            return TemperatureMin * Mathf.Pow(b, normalizedTemperature);
        }

        #endregion

        #region Radius

        /// <summary>
        /// Estimates the Radius of a star based on its Temperature and Luminosity.
        /// https://en.wikipedia.org/wiki/Stefan%E2%80%93Boltzmann_law#Examples
        /// </summary>
        /// <param name="temperature">Temperature in Kelvin.</param>
        /// <param name="luminosity">Luminosity.</param>
        /// <returns>The estimated radius in kilometers.</returns>
        public static float GetEstimatedRadius(float temperature, double luminosity)
        {
            double denominator = 4 * Math.PI * StefanBoltzmannConstant * Math.Pow(temperature, 4);
            return (float)(Math.Sqrt(luminosity / denominator) / 1000);
        }

        #endregion
    }
}

