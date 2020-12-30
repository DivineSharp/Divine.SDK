using System;

using SharpDX;

namespace Divine.SDK.Extensions
{
    public static class SharpDXExtensions
    {
        public static Vector2 FromPolarCoordinates(float radial, float polar)
        {
            return new Vector2((float)Math.Cos(polar) * radial, (float)Math.Sin(polar) * radial);
        }
    }
}