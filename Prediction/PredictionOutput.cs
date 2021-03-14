using System.Collections.Generic;

using Divine.SDK.Prediction.Collision;

using SharpDX;

namespace Divine.SDK.Prediction
{
    public class PredictionOutput
    {
        public IReadOnlyList<PredictionOutput> AoeTargetsHit { get; set; } = new List<PredictionOutput>();

        public float ArrivalTime { get; set; }

        public Vector3 CastPosition { get; set; }

        public CollisionResult CollisionResult { get; set; }

        public HitChance HitChance { get; set; }

        public Unit Unit { get; set; }

        public Vector3 UnitPosition { get; set; }
    }
}