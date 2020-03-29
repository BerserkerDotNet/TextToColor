using System;
using System.Drawing;

namespace TextToColor
{
    public static class TextToColor
    {
        public static HSLA ToHsl(this string text, Action<TextToColorConfiguration> configurator = null)
        {
            var config = TextToColorConfiguration.Default;
            if (configurator is object)
            {
                configurator(config);
            }

            ulong h;
            float s, l;
            var hash = config.HashProvider.Hash(text);
            h = hash % 359;
            hash = hash / 360;

            var saturationIndex = hash % (ulong)config.SaturationPercentages.Length;
            s = config.SaturationPercentages[saturationIndex];
            hash = hash / (ulong)config.SaturationPercentages.Length;
            var lightnessIndex = hash % (ulong)config.LightnessPercentages.Length;
            l = config.LightnessPercentages[lightnessIndex];

            return new HSLA(h, s, l, config.Alpha);
        }

        public static Color ToColor(this string text, Action<TextToColorConfiguration> configurator = null)
        {
            return ToHsl(text, configurator).ToColor();
        }
    }
}