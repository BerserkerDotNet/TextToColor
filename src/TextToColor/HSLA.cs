using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace TextToColor
{
    [DebuggerDisplay("HSLA = ({H}, {S}, {L}, {A})")]
    public class HSLA
    {
        public HSLA(ulong h, float s, float l, float alpha)
        {
            H = h;
            S = s;
            L = l;
            A = alpha;
        }

        public ulong H { get; set; }

        public float S { get; set; }

        public float L { get; set; }

        public float A { get; set; }

        public Color ToColor()
        {
            float h = H / 360f;

            float q = L < 0.5f ? L * (1f + S) : L + S - L * S;
            float p = 2f * L - q;

            var ranges = new float[] { h + 1f / 3f, h, h - 1f / 3f };

            var colors = ranges.Select(color =>
            {
                if (color < 0)
                {
                    color++;
                }

                if (color > 1)
                {
                    color--;
                }

                if (color < 1f / 6f)
                {
                    color = p + (q - p) * 6f * color;
                }
                else if (color < 0.5f)
                {
                    color = q;
                }
                else if (color < 2f / 3f)
                {
                    color = p + (q - p) * 6f * (2f / 3f - color);
                }
                else
                {
                    color = p;
                }

                return (byte)Math.Round((double)color * 255);
            }).ToArray();

            return Color.FromArgb((int)(A * 255), colors[0], colors[1], colors[2]);
        }
    }
}
