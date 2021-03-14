using System;
using System.Collections.Generic;
using System.Linq;

using Divine.SDK.Extensions;
using Divine.SDK.Prediction.Collision;

using SharpDX;

namespace Divine.SDK.Prediction
{
    public sealed class Prediction : IPrediction
    {
        public PredictionOutput GetPrediction(PredictionInput input)
        {
            var result = this.GetSimplePrediction(input);

            // Handle AreaOfEffect
            if (input.AreaOfEffect)
            {
                this.GetAreaOfEffectPrediction(input, result);
            }

            this.GetProperCastPosition(input, result);

            // check range
            if (input.Range != float.MaxValue)
            {
                if (!this.IsInRange(input, result.CastPosition, false) && !this.IsInRange(input, result.UnitPosition, true))
                {
                    result.HitChance = HitChance.OutOfRange;
                }
            }

            // check collision
            if (input.CollisionTypes != CollisionTypes.None)
            {
                var scanRange = input.Owner.Distance2D(result.CastPosition);
                var movingObjects = new List<Unit>();
                var collisionObjects = new List<CollisionObject>();

                if ((input.CollisionTypes & CollisionTypes.AllyCreeps) == CollisionTypes.AllyCreeps)
                {
                    movingObjects.AddRange(
                        EntityManager.GetEntities<Creep>().Where(
                            unit => unit.IsAlly(input.Owner) && unit.IsValidTarget(float.MaxValue, false) && input.Owner.IsInRange(unit, scanRange) 
                                    && unit != input.Target));
                }

                if ((input.CollisionTypes & CollisionTypes.EnemyCreeps) == CollisionTypes.EnemyCreeps)
                {
                    movingObjects.AddRange(
                        EntityManager.GetEntities<Creep>().Where(
                            unit => unit.IsEnemy(input.Owner) && unit.IsValidTarget(float.MaxValue, false) && input.Owner.IsInRange(unit, scanRange)
                                    && unit != input.Target));
                }

                if ((input.CollisionTypes & CollisionTypes.AllyHeroes) == CollisionTypes.AllyHeroes)
                {
                    movingObjects.AddRange(
                        EntityManager.GetEntities<Hero>().Where(
                            unit => unit.IsAlly(input.Owner)
                                    && unit.IsValidTarget(float.MaxValue, false)
                                    && input.Owner.IsInRange(unit, scanRange)
                                    && unit != input.Owner
                                    && unit != input.Target));
                }

                if ((input.CollisionTypes & CollisionTypes.EnemyHeroes) == CollisionTypes.EnemyHeroes)
                {
                    movingObjects.AddRange(
                        EntityManager.GetEntities<Hero>().Where(
                            unit => unit.IsEnemy(input.Owner)
                                    && unit.IsValidTarget(float.MaxValue, false)
                                    && input.Owner.IsInRange(unit, scanRange)
                                    && unit != input.Target));
                }

                // add units
                foreach (var unit in movingObjects)
                {
                    var predictedPos = this.GetSimplePrediction(input.WithTarget(unit));
                    collisionObjects.Add(new CollisionObject(unit, predictedPos.UnitPosition, unit.HullRadius + 10f));
                    collisionObjects.Add(new CollisionObject(unit, unit.Position, unit.HullRadius)); // optional
                }

                // add trees and buildings, use NavMeshCellFlags for less lag?
                if ((input.CollisionTypes & CollisionTypes.Trees) == CollisionTypes.Trees)
                {
                    foreach (var tree in EntityManager.GetEntities<Tree>().Where(unit => input.Owner.IsInRange(unit, scanRange)))
                    {
                        collisionObjects.Add(new CollisionObject(tree, tree.Position, 75f));
                    }
                }

                // runes for pudge
                if ((input.CollisionTypes & CollisionTypes.Runes) == CollisionTypes.Runes)
                {
                    foreach (var rune in EntityManager.GetEntities<Rune>().Where(unit => input.Owner.IsInRange(unit, scanRange)))
                    {
                        collisionObjects.Add(new CollisionObject(rune, rune.Position, 75f));
                    }
                }

                var collisionResult = Collision.Collision.GetCollision(input.Owner.Position.ToVector2(), result.CastPosition.ToVector2(), input.Radius, collisionObjects);
                if (collisionResult.Collides)
                {
                    result.HitChance = HitChance.Collision;
                }

                result.CollisionResult = collisionResult;
            }

            return result;
        }

        private static PredictionOutput PredictionOutput(Unit target, Vector3 position, HitChance hitChance)
        {
            return new PredictionOutput
            {
                Unit = target,
                CastPosition = position,
                UnitPosition = position,
                HitChance = hitChance
            };
        }

        private Vector3 ExtendUntilWall(Vector3 start, Vector3 direction, float distance)
        {
            var step = MapManager.MeshCellSize / 2f;
            var testPoint = start;
            var sign = distance > 0f ? 1f : -1f;

            distance = Math.Abs(distance);

            while (distance > 0f && (MapManager.GetMeshCellFlags(testPoint) & MapMeshCellFlags.Walkable) == MapMeshCellFlags.Walkable)
            {
                if (step > distance)
                {
                    step = distance;
                }

                testPoint += (sign * direction * step);
                distance -= step;
            }

            return testPoint;
        }

        private void GetAreaOfEffectPrediction(PredictionInput input, PredictionOutput output)
        {
            var targets = new List<PredictionOutput>();

            // aoe targets
            foreach (var target in input.AreaOfEffectTargets.Where(e => e.Handle != output.Unit.Handle))
            {
                var targetPrediction = this.GetSimplePrediction(input.WithTarget(target));

                if (this.IsInRange(input, targetPrediction.UnitPosition))
                {
                    targets.Add(targetPrediction);
                }
            }

            switch (input.PredictionSkillshotType)
            {
                case PredictionSkillshotType.SkillshotCircle:

                    if (input.AreaOfEffectHitMainTarget)
                    {
                        // main target
                        targets.Insert(0, output);

                        while (targets.Count > 1)
                        {
                            var mecResult = MEC.GetMec(targets.Select((target) => target.UnitPosition.ToVector2()).ToList());

                            // add hullradius?
                            if (mecResult.Radius != 0f && mecResult.Radius < input.Radius && this.IsInRange(input, mecResult.Center.ToVector3()))
                            {
                                output.CastPosition = new Vector3(
                                    targets.Count <= 2 ? (targets[0].UnitPosition.ToVector2() + targets[1].UnitPosition.ToVector2()) / 2 : mecResult.Center,
                                    output.CastPosition.Z);
                                output.AoeTargetsHit = targets.Where((target) => output.CastPosition.IsInRange(target.UnitPosition, input.Radius)).ToList();
                                break;
                            }

                            var itemToRemove = targets.MaxOrDefault((target) => targets[0].UnitPosition.DistanceSquared(target.UnitPosition));
                            targets.Remove(itemToRemove);
                        }
                    }
                    else
                    {
                        // TODO: handle the AreaOfEffectHitMainTarget=false case
                    }

                    break;

                case PredictionSkillshotType.SkillshotCone:
                    break;

                case PredictionSkillshotType.SkillshotLine:
                    break;
            }
        }

        private void GetProperCastPosition(PredictionInput input, PredictionOutput output)
        {
            var radius = input.Radius;

            if (radius <= 0)
            {
                return;
            }

            var caster = input.Owner;
            var casterPosition = caster.Position;
            var castPosition = output.CastPosition;
            var distance = casterPosition.ToVector2().Distance(castPosition);
            var range = input.Range;

            if (range >= distance)
            {
                return;
            }

            castPosition = castPosition.Extend(casterPosition, Math.Min(distance - range, radius));

            if (output.AoeTargetsHit.Any())
            {
                var maxDistance = output.AoeTargetsHit.Max(x => x.UnitPosition.ToVector2().Distance(castPosition));
                if (maxDistance > radius)
                {
                    distance = casterPosition.ToVector2().Distance(castPosition);
                    castPosition = casterPosition.Extend(castPosition, distance + (maxDistance - radius));
                }
            }

            output.CastPosition = castPosition;
        }

        private PredictionOutput GetSimplePrediction(PredictionInput input)
        {
            var target = input.Target;
            var targetPosition = input.Target.Position;
            var caster = input.Owner;

            var totalDelay = caster.TurnTime(targetPosition) + input.Delay + (GameManager.Ping / 1000f) + 0.06f;
            var totalArrivalTime = totalDelay + (caster.Distance2D(target, true) / input.Speed);

            if (target.NetworkActivity != NetworkActivity.Move)
            {
                if (target.IsStunned() || target.IsRooted())
                {
                    // TODO: test this immobile duration
                    // var immobileDuration = target.ImmobileDuration();

                    //// check if enemy could run out of the radius
                    // if (totalArrivalTime - (input.Radius / target.MovementSpeed) > immobileDuration)
                    // {
                    // // assume enemy will run in their facing direction 
                    // return new PredictionOutput
                    // {
                    // Unit = input.Target,
                    // ArrivalTime = totalArrivalTime,
                    // UnitPosition = this.ExtendUntilWall(targetPosition, direction.ToVector3(),
                    // (totalArrivalTime - immobileDuration) * target.MovementSpeed),
                    // CastPosition =
                    // this.ExtendUntilWall(
                    // targetPosition,
                    // direction.ToVector3(),
                    // (((totalArrivalTime - immobileDuration) * target.MovementSpeed) + 20f) - input.Radius - (target.HullRadius / 2.0f)),
                    // HitChance = HitChance.Medium
                    // };
                    // }
                    return PredictionOutput(target, targetPosition, HitChance.Immobile);
                }

                if (!caster.IsVisibleToEnemies)
                {
                    return PredictionOutput(target, targetPosition, HitChance.High);
                }

                return PredictionOutput(target, targetPosition, HitChance.Medium);
            }

            var rotationDifferenceRad = (target.RotationDifference * (float)Math.PI) / 180f;
            var direction = rotationDifferenceRad != 0f ? Vector2Extensions.Rotated(target.Direction2D(), rotationDifferenceRad) : target.Direction2D();

            if (rotationDifferenceRad != 0f)
            {
                var timeToRotate = target.TurnTime(Math.Abs(rotationDifferenceRad));
                totalDelay -= timeToRotate;
                totalArrivalTime -= timeToRotate;
            }

            if (input.Speed != float.MaxValue)
            {
                var result = VectorMovementCollision(
                    targetPosition.ToVector2(),
                    this.ExtendUntilWall(targetPosition, target.Direction2D().ToVector3(), totalArrivalTime * target.MovementSpeed).ToVector2(),
                    target.MovementSpeed,
                    caster.Position.ToVector2(),
                    input.Speed,
                    totalDelay);

                if (result.Item2 != Vector2.Zero)
                {
                    totalArrivalTime = result.Item1 + totalDelay;
                    return new PredictionOutput
                    {
                        Unit = input.Target,
                        ArrivalTime = totalArrivalTime,
                        UnitPosition = this.ExtendUntilWall(targetPosition, direction.ToVector3(), totalArrivalTime * target.MovementSpeed),
                        CastPosition = this.ExtendUntilWall(targetPosition, direction.ToVector3(), ((totalArrivalTime * target.MovementSpeed) + 20f) - (target.HullRadius / 2.0f)),
                        HitChance = !caster.IsVisibleToEnemies ? HitChance.High : HitChance.Medium
                    };
                }
            }

            return new PredictionOutput
            {
                Unit = input.Target,
                ArrivalTime = totalArrivalTime,
                UnitPosition = this.ExtendUntilWall(targetPosition, direction.ToVector3(), totalArrivalTime * target.MovementSpeed),
                CastPosition = this.ExtendUntilWall(targetPosition, direction.ToVector3(), ((totalArrivalTime * target.MovementSpeed) + 20f) - (target.HullRadius / 2.0f)),
                HitChance = input.Speed != float.MaxValue ? HitChance.Low : HitChance.Medium
            };
        }

        private bool IsInRange(PredictionInput input, Vector3 position, bool addRadius = true)
        {
            return input.Owner.IsInRange(position, input.Range + (addRadius ? input.Radius : 0f));
        }

        private static Tuple<float, Vector2> VectorMovementCollision(Vector2 startPoint1, Vector2 endPoint1, float v1, Vector2 startPoint2, float v2, float delay = 0f)
        {
            float sP1x = startPoint1.X, sP1y = startPoint1.Y, eP1x = endPoint1.X, eP1y = endPoint1.Y, sP2x = startPoint2.X, sP2y = startPoint2.Y;

            float d = eP1x - sP1x, e = eP1y - sP1y;
            float dist = (float)Math.Sqrt((d * d) + (e * e)), t1 = float.NaN;
            float S = Math.Abs(dist) > float.Epsilon ? (v1 * d) / dist : 0, K = Math.Abs(dist) > float.Epsilon ? (v1 * e) / dist : 0f;

            float r = sP2x - sP1x, j = sP2y - sP1y;
            var c = (r * r) + (j * j);

            if (dist > 0f)
            {
                if (Math.Abs(v1 - float.MaxValue) < float.Epsilon)
                {
                    var t = dist / v1;
                    t1 = (v2 * t) >= 0f ? t : float.NaN;
                }
                else if (Math.Abs(v2 - float.MaxValue) < float.Epsilon)
                {
                    t1 = 0f;
                }
                else
                {
                    float a = ((S * S) + (K * K)) - (v2 * v2), b = (-r * S) - (j * K);

                    if (Math.Abs(a) < float.Epsilon)
                    {
                        if (Math.Abs(b) < float.Epsilon)
                        {
                            t1 = Math.Abs(c) < float.Epsilon ? 0f : float.NaN;
                        }
                        else
                        {
                            var t = -c / (2 * b);
                            t1 = (v2 * t) >= 0f ? t : float.NaN;
                        }
                    }
                    else
                    {
                        var sqr = (b * b) - (a * c);
                        if (sqr >= 0)
                        {
                            var nom = (float)Math.Sqrt(sqr);
                            var t = (-nom - b) / a;
                            t1 = (v2 * t) >= 0f ? t : float.NaN;
                            t = (nom - b) / a;
                            var t2 = (v2 * t) >= 0f ? t : float.NaN;

                            if (!float.IsNaN(t2) && !float.IsNaN(t1))
                            {
                                if (t1 >= delay && t2 >= delay)
                                {
                                    t1 = Math.Min(t1, t2);
                                }
                                else if (t2 >= delay)
                                {
                                    t1 = t2;
                                }
                            }
                        }
                    }
                }
            }
            else if (Math.Abs(dist) < float.Epsilon)
            {
                t1 = 0f;
            }

            return new Tuple<float, Vector2>(t1, !float.IsNaN(t1) ? new Vector2(sP1x + (S * t1), sP1y + (K * t1)) : Vector2.Zero);
        }
    }
}