using System;
using System.Collections.Generic;
using System.Linq;

using SharpDX;

namespace Divine.SDK.Extensions
{
    public static class Vector4Extensions
    {
        public static float AngleBetween(this Vector4 vector4, Vector4 toVector4)
        {
            return AngleBetween(vector4, toVector4.ToVector3());
        }

        public static float AngleBetween(this Vector4 vector4, Vector2 toVector2)
        {
            return AngleBetween(vector4, toVector2.ToVector3());
        }

        public static float AngleBetween(this Vector4 vector4, Vector3 toVector3)
        {
            var theta = vector4.Polar() - toVector3.Polar();
            if (theta < 0)
            {
                theta += 360;
            }

            if (theta > 180)
            {
                theta = 360 - theta;
            }

            return theta;
        }

        public static Vector4 Closest(this Vector4 vector4, IEnumerable<Vector4> array)
        {
            var result = Vector4.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector4.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static Vector3 Closest(this Vector4 vector4, IEnumerable<Vector3> array)
        {
            var result = Vector3.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector4.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static Vector2 Closest(this Vector4 vector4, IEnumerable<Vector2> array)
        {
            var result = Vector2.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector4.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static float Distance(this Vector4 vector4, Vector4 toVector4)
        {
            return Vector4.Distance(vector4, toVector4);
        }

        public static float Distance(this Vector4 vector4, Vector2 toVector2)
        {
            return Vector4.Distance(vector4, toVector2.ToVector4());
        }

        public static float Distance(this Vector4 vector4, Vector3 toVector3)
        {
            return Vector4.Distance(vector4, toVector3.ToVector4());
        }

        public static float DistanceSquared(this Vector4 vector4, Vector4 toVector4)
        {
            return Vector4.DistanceSquared(vector4, toVector4);
        }

        public static float DistanceSquared(this Vector4 vector4, Vector2 toVector2)
        {
            return Vector4.DistanceSquared(vector4, toVector2.ToVector4());
        }

        public static float DistanceSquared(this Vector4 vector4, Vector3 toVector3)
        {
            return Vector4.DistanceSquared(vector4, toVector3.ToVector4());
        }

        public static Vector4 Extend(this Vector4 vector4, Vector4 toVector4, float distance)
        {
            return vector4 + (distance * (toVector4 - vector4).Normalized());
        }

        public static Vector4 Extend(this Vector4 vector4, Vector2 toVector2, float distance)
        {
            return vector4 + (distance * (toVector2.ToVector4(vector4.Z) - vector4).Normalized());
        }

        public static Vector4 Extend(this Vector4 vector4, Vector3 toVector3, float distance)
        {
            return vector4 + (distance * (toVector3.ToVector4() - vector4).Normalized());
        }

        public static bool IsOnScreen(this Vector4 vector4)
        {
            return vector4.ToVector3().IsOnScreen();
        }

        public static bool IsOrthogonal(this Vector4 vector4, Vector4 toVector4)
        {
            return IsOrthogonal(vector4, toVector4.ToVector3());
        }

        public static bool IsOrthogonal(this Vector4 vector4, Vector2 toVector2)
        {
            return IsOrthogonal(vector4, toVector2.ToVector3());
        }

        public static bool IsOrthogonal(this Vector4 vector4, Vector3 toVector3)
        {
            return Math.Abs((vector4.X * toVector3.X) + (vector4.Y * toVector3.Y)) < float.Epsilon;
        }

        public static bool IsValid(this Vector4 vector4)
        {
            return vector4 != Vector4.Zero;
        }

        public static bool IsWall(this Vector4 vector4)
        {
            return vector4.ToVector3().IsWall();
        }

        public static float Magnitude(this Vector4 vector4)
        {
            return (float)Math.Sqrt((vector4.X * vector4.X) + (vector4.Y * vector4.Y) + (vector4.Z * vector4.Z));
        }

        public static Vector4 Normalized(this Vector4 vector4)
        {
            vector4.Normalize();
            return vector4;
        }

        public static float PathLength(this List<Vector4> path)
        {
            var distance = 0f;
            for (var i = 0; i < (path.Count - 1); i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        public static Vector4 Perpendicular(this Vector4 vector4, int offset = 0)
        {
            return offset == 0 ? new Vector4(-vector4.Y, vector4.X, vector4.Z, vector4.W) : new Vector4(vector4.Y, -vector4.X, vector4.Z, vector4.W);
        }

        public static float Polar(this Vector4 vector4)
        {
            if (Math.Abs(vector4.X - 0) <= (float)1e-9)
            {
                return vector4.Y > 0 ? 90 : vector4.Y < 0 ? 270 : 0;
            }

            var theta = (float)(Math.Atan(vector4.Y / vector4.X) * (180 / Math.PI));
            if (vector4.X < 0)
            {
                theta += 180;
            }

            if (theta < 0)
            {
                theta += 360;
            }

            return theta;
        }

        public static Vector4 Rotated(this Vector4 vector4, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new Vector4((float)((vector4.X * cos) - (vector4.Y * sin)), (float)((vector4.Y * cos) + (vector4.X * sin)), vector4.Z, vector4.W);
        }

        public static Vector4 SetW(this Vector4 v, float? value = null)
        {
            if (value == null)
            {
                v.W = 1.0f;
            }
            else
            {
                v.W = (float)value;
            }

            return v;
        }

        public static Vector4 SetZ(this Vector4 v, float? value = null)
        {
            if (value == null)
            {
                v.Z = GameManager.MousePosition.Z;
            }
            else
            {
                v.Z = (float)value;
            }

            return v;
        }

        public static Vector2 ToVector2(this Vector4 vector4)
        {
            return new Vector2(vector4.X, vector4.Y);
        }

        public static List<Vector2> ToVector2(this List<Vector4> path)
        {
            return path.Select(point => point.ToVector2()).ToList();
        }

        public static Vector3 ToVector3(this Vector4 vector4)
        {
            return new Vector3(vector4.X, vector4.Y, vector4.Z);
        }

        public static List<Vector3> ToVector3(this List<Vector4> path)
        {
            return path.Select(point => point.ToVector3()).ToList();
        }
    }
}