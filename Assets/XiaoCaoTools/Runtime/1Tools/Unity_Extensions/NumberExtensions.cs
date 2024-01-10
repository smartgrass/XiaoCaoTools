using System;
using UnityEngine;

namespace GG.Extensions
{
    public static class NumberExtensions
    {
        public enum RoundMode
        {
            UpDown,
            Up,
            Down
        }

        public static int NearestMultipleOf(this int x, int multiple, RoundMode rm = RoundMode.UpDown)
        {
            return Mathf.RoundToInt(((float)x).NearestMultipleOf((float)multiple, rm));
        }

        public static float NearestMultipleOf(this float x, float multiple, RoundMode rm = RoundMode.UpDown)
        {
            float mod = x % multiple;
            float midPoint = multiple / 2.0f;

            if (rm == RoundMode.UpDown)
            {
                if (mod > midPoint)
                {
                    return x + (multiple - mod);
                }
                else
                {
                    return x - mod;
                }
            }
            else if (rm == RoundMode.Up)
            {
                return x + (multiple - mod);
            }
            else//(rm == RoundMode.Down)
            {
                return x - mod;
            }
        }

        public static bool FallsBetween(this float numberToCheck, float bottom, float top)
        {
            return (numberToCheck >= bottom && numberToCheck <= top);
        }
    }
}
