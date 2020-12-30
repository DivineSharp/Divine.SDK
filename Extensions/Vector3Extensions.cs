using System;
using System.Collections.Generic;
using System.Linq;

using SharpDX;

namespace Divine.SDK.Extensions
{
    public static class Vector3Extensions
    {
        public static float AngleBetween(this Vector3 vector3, Vector3 toVector3)
        {
            var theta = vector3.Polar() - toVector3.Polar();
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

        public static float AngleBetween(this Vector3 vector3, Vector2 toVector2)
        {
            return AngleBetween(vector3, toVector2.ToVector3());
        }

        public static float AngleBetween(this Vector3 vector3, Vector4 toVector4)
        {
            return AngleBetween(vector3, toVector4.ToVector3());
        }

        public static Vector3 Closest(this Vector3 vector3, IEnumerable<Vector3> array)
        {
            var result = Vector3.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector3.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static Vector2 Closest(this Vector3 vector3, IEnumerable<Vector2> array)
        {
            var result = Vector2.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector3.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static Vector4 Closest(this Vector3 vector3, IEnumerable<Vector4> array)
        {
            var result = Vector4.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector3.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static float Distance(this Vector3 vector3, Vector3 toVector3)
        {
            return Vector3.Distance(vector3, toVector3);
        }

        public static float Distance(this Vector3 vector3, Vector2 toVector2)
        {
            return Vector3.Distance(vector3, toVector2.ToVector3());
        }

        public static float Distance(this Vector3 vector3, Vector4 toVector4)
        {
            return Vector3.Distance(vector3, toVector4.ToVector3());
        }

        public static float DistanceSquared(this Vector3 vector3, Vector3 toVector3)
        {
            return Vector3.DistanceSquared(vector3, toVector3);
        }

        public static float DistanceSquared(this Vector3 vector3, Vector2 toVector2)
        {
            return Vector3.DistanceSquared(vector3, toVector2.ToVector3());
        }

        public static float DistanceSquared(this Vector3 vector3, Vector4 toVector4)
        {
            return Vector3.DistanceSquared(vector3, toVector4.ToVector3());
        }

        public static Vector3 Extend(this Vector3 vector3, Vector3 toVector3, float distance)
        {
            return vector3 + (distance * (toVector3 - vector3).Normalized());
        }

        public static Vector3 Extend(this Vector3 vector3, Vector2 toVector2, float distance)
        {
            return vector3 + (distance * (toVector2.ToVector3(vector3.Z) - vector3).Normalized());
        }

        public static Vector3 Extend(this Vector3 vector3, Vector4 toVector4, float distance)
        {
            return vector3 + (distance * (toVector4.ToVector3() - vector3).Normalized());
        }

        public static float GetPathLength(this List<Vector3> path)
        {
            var distance = 0f;

            for (var i = 0; i < (path.Count - 1); i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        public static bool IsInRange(this Vector3 sourcePosition, Entity target, float range)
        {
            return target.Position.IsInRange(sourcePosition, range);
        }

        public static bool IsInRange(this Vector3 sourcePosition, Vector2 targetPosition, float range)
        {
            var diffX = sourcePosition.X - targetPosition.X;
            var diffY = sourcePosition.Y - targetPosition.Y;

            return ((diffX * diffX) + (diffY * diffY)) < (range * range);
        }

        public static bool IsInRange(this Vector3 sourcePosition, Vector3 targetPosition, float range)
        {
            var diffX = sourcePosition.X - targetPosition.X;
            var diffY = sourcePosition.Y - targetPosition.Y;

            return ((diffX * diffX) + (diffY * diffY)) < (range * range);
        }

        public static bool IsOnScreen(this Vector3 vector3)
        {
            var screenPosition = RendererManager.WorldToScreen(vector3);
            var screenSize = RendererManager.ScreenSize;

            return screenPosition.X > 0 && screenPosition.X <= screenSize.X && screenPosition.Y > 0 && screenPosition.Y <= screenSize.Y;
        }

        public static bool IsOrthogonal(Vector3 vector3, Vector3 toVector3)
        {
            return Math.Abs((vector3.X * toVector3.X) + (vector3.Y * toVector3.Y)) < float.Epsilon;
        }

        public static bool IsOrthogonal(Vector3 vector3, Vector2 toVector2)
        {
            return IsOrthogonal(vector3, toVector2.ToVector3());
        }

        public static bool IsOrthogonal(Vector3 vector3, Vector4 toVector4)
        {
            return IsOrthogonal(vector3, toVector4.ToVector3());
        }

        public static bool IsValid(this Vector3 vector3)
        {
            return vector3 != Vector3.Zero;
        }

        public static bool IsWall(this Vector3 vector3)
        {
            return false; // NavMesh.GetCollisionFlags(vector3).HasFlag(CollisionFlags.Wall);
        }

        public static float Magnitude(this Vector3 vector3)
        {
            return (float)Math.Sqrt((vector3.X * vector3.X) + (vector3.Y * vector3.Y) + (vector3.Z * vector3.Z));
        }

        public static Vector3 Normalized(this Vector3 vector3)
        {
            vector3.Normalize();
            return vector3;
        }

        public static float PathLength(this List<Vector3> path)
        {
            var distance = 0f;
            for (var i = 0; i < (path.Count - 1); i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        public static Vector3 Perpendicular(this Vector3 vector3, int offset = 0)
        {
            return offset == 0 ? new Vector3(-vector3.Y, vector3.X, vector3.Z) : new Vector3(vector3.Y, -vector3.X, vector3.Z);
        }

        public static float Polar(this Vector3 vector3)
        {
            if (Math.Abs(vector3.X - 0) <= (float)1e-9)
            {
                return vector3.Y > 0 ? 90 : vector3.Y < 0 ? 270 : 0;
            }

            var theta = (float)(Math.Atan(vector3.Y / vector3.X) * (180 / Math.PI));
            if (vector3.X < 0)
            {
                theta += 180;
            }

            if (theta < 0)
            {
                theta += 360;
            }

            return theta;
        }

        public static Vector3 Rotated(this Vector3 vector3, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new Vector3((float)((vector3.X * cos) - (vector3.Y * sin)), (float)((vector3.Y * cos) + (vector3.X * sin)), vector3.Z);
        }

        public static Vector3 SetZ(this Vector3 v, float? value = null)
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

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }

        public static List<Vector2> ToVector2(this List<Vector3> path)
        {
            return path.Select(point => point.ToVector2()).ToList();
        }

        public static Vector4 ToVector4(this Vector3 vector3, float w = 1f)
        {
            return new Vector4(vector3, w);
        }

        public static List<Vector4> ToVector4(this List<Vector3> path)
        {
            return path.Select(point => point.ToVector4()).ToList();
        }
    }
}