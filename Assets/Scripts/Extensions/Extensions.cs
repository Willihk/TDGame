using System;
using UnityEngine;

namespace TDGame.Extensions
{
    public static class Extensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            return val.CompareTo(max) > 0 ? max : val;
        }

        public static float ToRadians(this int degress)
        {
            return degress * 180 / Mathf.PI;
        }

        public static float ToDegress(this int degress)
        {
            return degress * Mathf.PI / 180;
        }
    }
}