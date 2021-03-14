using System;
using System.Collections.Generic;
using System.Linq;

using SharpDX;

namespace Divine.SDK.Extensions
{
    public static class Vector2Extensions
    {
        public static float AngleBetween(this Vector2 vector2, Vector2 toVector2)
        {
            return AngleBetween(vector2, toVector2.ToVector3());
        }

        public static float AngleBetween(this Vector2 vector2, Vector3 toVector3)
        {
            var theta = vector2.Polar() - toVector3.Polar();
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

        public static float AngleBetween(this Vector2 vector2, Vector4 toVector4)
        {
            return AngleBetween(vector2, toVector4.ToVector3());
        }

        public static Vector2[] CircleCircleIntersection(this Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var d = center1.Distance(center2);

            if (d > (radius1 + radius2) || d <= Math.Abs(radius1 - radius2))
            {
                return new Vector2[]
                       {
                       };
            }

            var a = (((radius1 * radius1) - (radius2 * radius2)) + (d * d)) / (2 * d);
            var h = (float)Math.Sqrt((radius1 * radius1) - (a * a));
            var direction = (center2 - center1).Normalized();
            var pa = center1 + (a * direction);
            var s1 = pa + (h * direction.Perpendicular());
            var s2 = pa - (h * direction.Perpendicular());
            return new[]
                   {
                       s1,
                       s2
                   };
        }

        public static Vector2 Closest(this Vector2 vector2, IEnumerable<Vector2> array)
        {
            var result = Vector2.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector2.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static Vector3 Closest(this Vector2 vector2, IEnumerable<Vector3> array)
        {
            var result = Vector3.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector2.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static Vector4 Closest(this Vector2 vector2, IEnumerable<Vector4> array)
        {
            var result = Vector4.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector2.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        public static float CrossProduct(this Vector2 self, Vector2 other)
        {
            return (other.Y * self.X) - (other.X * self.Y);
        }

        public static float Distance(this Vector2 vector2, Vector2 toVector2)
        {
            return Vector2.Distance(vector2, toVector2);
        }

        public static float Distance(this Vector2 vector2, Vector3 toVector3)
        {
            return Vector2.Distance(vector2, toVector3.ToVector2());
        }

        public static float Distance(this Vector2 vector2, Vector4 toVector4)
        {
            return Vector2.Distance(vector2, toVector4.ToVector2());
        }

        public static float Distance(this Vector2 vector2, Vector2 to, bool squared = false)
        {
            return squared ? Vector2.DistanceSquared(vector2, to) : Vector2.Distance(vector2, to);
        }

        public static float Distance(this Vector2 vector2, Vector3 to, bool squared = false)
        {
            return vector2.Distance(to.ToVector2(), squared);
        }

        public static float Distance(this Vector2 vector2, Vector2 segmentStart, Vector2 segmentEnd, bool onlyIfOnSegment = false, bool squared = false)
        {
            var objects = vector2.ProjectOn(segmentStart, segmentEnd);

            if (objects.IsOnSegment || onlyIfOnSegment == false)
            {
                return squared ? Vector2.DistanceSquared(objects.SegmentPoint, vector2) : Vector2.Distance(objects.SegmentPoint, vector2);
            }

            return float.MaxValue;
        }

        public static float DistanceSquared(this Vector2 vector2, Vector2 toVector2)
        {
            return Vector2.DistanceSquared(vector2, toVector2);
        }

        public static float DistanceSquared(this Vector2 vector2, Vector3 toVector3)
        {
            return Vector2.DistanceSquared(vector2, toVector3.ToVector2());
        }

        public static float DistanceSquared(this Vector2 vector2, Vector4 toVector4)
        {
            return Vector2.DistanceSquared(vector2, toVector4.ToVector2());
        }

        public static Vector2 Extend(this Vector2 vector2, Vector2 toVector2, float distance)
        {
            return vector2 + (distance * (toVector2 - vector2).Normalized());
        }

        public static Vector2 Extend(this Vector2 vector2, Vector3 toVector3, float distance)
        {
            return vector2 + (distance * (toVector3.ToVector2() - vector2).Normalized());
        }

        public static Vector2 Extend(this Vector2 vector2, Vector4 toVector4, float distance)
        {
            return vector2 + (distance * (toVector4.ToVector2() - vector2).Normalized());
        }

        public static float GetPathLength(this List<Vector2> path)
        {
            var distance = 0f;

            for (var i = 0; i < (path.Count - 1); i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        public static bool IsInRange(this Vector2 sourcePosition, Entity target, float range)
        {
            return target.Position.IsInRange(sourcePosition, range);
        }

        public static bool IsInRange(this Vector2 sourcePosition, Vector2 targetPosition, float range)
        {
            var diffX = sourcePosition.X - targetPosition.X;
            var diffY = sourcePosition.Y - targetPosition.Y;

            return ((diffX * diffX) + (diffY * diffY)) < (range * range);
        }

        public static bool IsInRange(this Vector2 sourcePosition, Vector3 targetPosition, float range)
        {
            var diffX = sourcePosition.X - targetPosition.X;
            var diffY = sourcePosition.Y - targetPosition.Y;

            return ((diffX * diffX) + (diffY * diffY)) < (range * range);
        }

        public static bool IsOnScreen(this Vector2 vector2)
        {
            return vector2.ToVector3().IsOnScreen();
        }

        public static bool IsOrthogonal(this Vector2 vector2, Vector2 toVector2)
        {
            return IsOrthogonal(vector2, toVector2.ToVector3());
        }

        public static bool IsOrthogonal(this Vector2 vector2, Vector3 toVector3)
        {
            return Math.Abs((vector2.X * toVector3.X) + (vector2.Y * toVector3.Y)) < float.Epsilon;
        }

        public static bool IsOrthogonal(this Vector2 vector2, Vector4 toVector4)
        {
            return IsOrthogonal(vector2, toVector4.ToVector3());
        }

        public static bool IsUnderRectangle(this Vector2 position, RectangleF rectangleF)
        {
            return position.IsUnderRectangle(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
        }

        public static bool IsUnderRectangle(this Vector2 position, float x, float y, float width, float height)
        {
            return position.X > x && position.X <= x + width && position.Y > y && position.Y <= y + height;
        }

        public static bool IsValid(this Vector2 vector2)
        {
            return vector2 != Vector2.Zero;
        }

        public static bool IsWall(this Vector2 vector2)
        {
            return vector2.ToVector3().IsWall();
        }

        public static float Magnitude(this Vector2 vector2)
        {
            return (float)Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y));
        }

        public static Vector2 Normalized(this Vector2 vector2)
        {
            vector2.Normalize();
            return vector2;
        }

        public static float PathLength(this List<Vector2> path)
        {
            var distance = 0f;
            for (var i = 0; i < (path.Count - 1); i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        public static Vector2 Perpendicular(this Vector2 vector2, int offset = 0)
        {
            return offset == 0 ? new Vector2(-vector2.Y, vector2.X) : new Vector2(vector2.Y, -vector2.X);
        }

        public static float Polar(this Vector2 vector2)
        {
            if (Math.Abs(vector2.X - 0) <= (float)1e-9)
            {
                return vector2.Y > 0 ? 90 : vector2.Y < 0 ? 270 : 0;
            }

            var theta = (float)(Math.Atan(vector2.Y / vector2.X) * (180 / Math.PI));
            if (vector2.X < 0)
            {
                theta += 180;
            }

            if (theta < 0)
            {
                theta += 360;
            }

            return theta;
        }

        public static Vector2 Rotated(this Vector2 vector2, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new Vector2((float)((vector2.X * cos) - (vector2.Y * sin)), (float)((vector2.Y * cos) + (vector2.X * sin)));
        }

        public static Vector3 ToVector3(this Vector2 vector2, float z = 0f)
        {
            return new Vector3(vector2, z);
        }

        public static List<Vector3> ToVector3(this List<Vector2> path)
        {
            return path.Select(point => point.ToVector3()).ToList();
        }

        public static Vector4 ToVector4(this Vector2 vector2, float z = 0f, float w = 1f)
        {
            return new Vector4(vector2, z, w);
        }

        public static List<Vector4> ToVector4(this List<Vector2> path)
        {
            return path.Select(point => point.ToVector4()).ToList();
        }

        public static ProjectionInfo ProjectOn(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            var cx = point.X;
            var cy = point.Y;
            var ax = segmentStart.X;
            var ay = segmentStart.Y;
            var bx = segmentEnd.X;
            var by = segmentEnd.Y;
            var rL = (((cx - ax) * (bx - ax)) + ((cy - ay) * (@by - ay))) / ((float)Math.Pow(bx - ax, 2) + (float)Math.Pow(by - ay, 2));
            var pointLine = new Vector2(ax + (rL * (bx - ax)), ay + (rL * (@by - ay)));
            float rS;
            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }

            var isOnSegment = rS.CompareTo(rL) == 0;
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + (rS * (bx - ax)), ay + (rS * (@by - ay)));
            return new ProjectionInfo(isOnSegment, pointSegment, pointLine);
        }

        public struct ProjectionInfo
        {
            public bool IsOnSegment;

            public Vector2 LinePoint;

            public Vector2 SegmentPoint;

            public ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
            {
                IsOnSegment = isOnSegment;
                SegmentPoint = segmentPoint;
                LinePoint = linePoint;
            }
        }
    }
}