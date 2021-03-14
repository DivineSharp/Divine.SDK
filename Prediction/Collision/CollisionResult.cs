using System.Collections.Generic;

namespace Divine.SDK.Prediction.Collision
{
    public class CollisionResult
    {
        public CollisionResult(List<CollisionObject> collisionObjects)
        {
            this.CollisionObjects = collisionObjects;
        }

        public bool Collides
        {
            get
            {
                return this.CollisionObjects.Count > 0;
            }
        }

        public IReadOnlyCollection<CollisionObject> CollisionObjects { get; }
    }
}