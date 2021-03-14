using System;

using SharpDX;

namespace Divine.SDK.Extensions
{
    public static class EntityExtensions
    {
        public static float Distance2D(this Unit entity, Unit other, bool fromCenterToCenter = false)
        {
            return entity.Distance2D(other.Position) - (fromCenterToCenter ? 0f : entity.HullRadius + other.HullRadius);
        }

        public static float Distance2D(this Entity entity, Entity other)
        {
            return entity.Distance2D(other.Position);
        }

        public static float Distance2D(this Entity entity, Vector3 position)
        {
            var entityPosition = entity.Position;
            return (float)Math.Sqrt(Math.Pow(entityPosition.X - position.X, 2) + Math.Pow(entityPosition.Y - position.Y, 2));
        }

        public static bool IsInRange(this Entity source, Entity target, float range)
        {
            return source.Position.IsInRange(target, range);
        }

        public static bool IsInRange(this Entity source, Vector2 targetPosition, float range)
        {
            return source.Position.IsInRange(targetPosition, range);
        }

        public static bool IsInRange(this Entity source, Vector3 targetPosition, float range)
        {
            return source.Position.IsInRange(targetPosition, range);
        }

        public static bool IsAlly(this Entity source, Entity target)
        {
            return source.Team == target.Team;
        }
    }
}