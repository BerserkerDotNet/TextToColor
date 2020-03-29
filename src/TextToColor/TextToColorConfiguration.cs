using System;
using System.Linq;

namespace TextToColor
{
    public class TextToColorConfiguration
    {
        private TextToColorConfiguration()
        {
        }

        public float[] SaturationPercentages { get; private set; }

        public float[] LightnessPercentages { get; private set; }

        public IHashProvider HashProvider { get; private set; }

        public float Alpha { get; private set; }

        public static TextToColorConfiguration Default => new TextToColorConfiguration()
            .WithMD5HashProvider()
            .WithPossibleSaturationValues(new[] { 0.35f, 0.5f, 0.65f })
            .WithPossibleLightnessValues(new[] { 0.35f, 0.5f, 0.65f })
            .WithAlpha(1f);

        /// <summary>
        /// Defines an array of values between 0f and 1f that represent a resulting saturation value
        /// </summary>
        /// <param name="saturationPercentages">Array of percentages </param>
        /// <returns></returns>
        public TextToColorConfiguration WithPossibleSaturationValues(params float[] saturationPercentages)
        {
            if (saturationPercentages == null || saturationPercentages.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(saturationPercentages), "Must have at least one value");
            }

            if (saturationPercentages.Any(v => v < 0 || v > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(saturationPercentages), "Values must in range from 0f to 1f");
            }

            SaturationPercentages = saturationPercentages;

            return this;
        }

        /// <summary>
        /// Defines an array of values between 0f and 1f that represent a resulting saturation value
        /// </summary>
        /// <param name="saturationPercentages"></param>
        /// <returns></returns>
        public TextToColorConfiguration WithPossibleLightnessValues(params float[] lightnessPercentages)
        {
            if (lightnessPercentages == null || lightnessPercentages.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lightnessPercentages), "Must have at least one value");
            }

            if (lightnessPercentages.Any(v => v < 0 || v > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(lightnessPercentages), "Values must be in range from 0f to 1f");
            }

            LightnessPercentages = lightnessPercentages;

            return this;
        }

        public TextToColorConfiguration WithSHA256HashProvider()
        {
            HashProvider = new SHA256HashProvider();
            return this;
        }

        public TextToColorConfiguration WithMD5HashProvider()
        {
            HashProvider = new MD5HashProvider();
            return this;
        }

        public TextToColorConfiguration WithHashProvider(IHashProvider provider)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider), "Hash provider cannot be null");
            }

            HashProvider = provider;
            return this;
        }

        public TextToColorConfiguration WithAlpha(float value)
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "alpha must be in range from 0f to 1f");
            }

            Alpha = value;
            return this;
        }
    }
}
