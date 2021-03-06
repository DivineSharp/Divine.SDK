using System;
using System.Collections.Generic;
using System.Linq;

using Divine.SDK.Managers.Log;

using SharpDX;

namespace Divine.SDK.Extensions
{
    public static class UnitExtensions
    {
        public static string GetDisplayName(this Unit unit)
        {
            var hero = unit as Hero;
            if (hero != null)
            {
                return hero.GetDisplayName();
            }

            var networkName = unit.NetworkName;
            return networkName.Substring("CDOTA_Unit_".Length).Replace("_", string.Empty);
        }

        public static float AttackPoint(this Unit unit)
        {
            try
            {
                var attackAnimationPoint = ((unit is Hero) ? Hero.GetKeyValueByName(unit.Name) : Unit.GetKeyValueByName(unit.Name)).GetKeyValue("AttackAnimationPoint").GetSingle();
                return attackAnimationPoint / (1f + ((unit.AttackSpeedValue() - 100f) / 100f));
            }
            catch
            {
                LogManager.Warn($"Missing AttackAnimationPoint for {unit.Name}");
                return 0;
            }
        }

        public static Modifier GetModifierByName(this Unit unit, string name)
        {
            return unit.Modifiers.FirstOrDefault(x => x.Name == name);
        }

        public static Modifier GetModifierByTextureName(this Unit unit, string name)
        {
            return unit.Modifiers.FirstOrDefault(x => x.TextureName == name);
        }

        public static float AttackRange(this Unit unit, Unit target = null)
        {
            var result = unit.AttackRange + unit.HullRadius;

            if (target != null)
            {
                result += target.HullRadius;
            }

            if (unit is Creep)
            {
                result += 15f;
            }

            var hero = unit as Hero;
            if (hero != null)
            {
                if (hero.IsRanged)
                {
                    // test for talents with bonus range
                    foreach (var ability in hero.Spellbook.Spells.Where(x => x.Level > 0 && x.Name.StartsWith("special_bonus_attack_range_")))
                    {
                        result += ability.GetAbilitySpecialData("value");
                    }

                    // test for items with bonus range
                    var bonusRangeItem = hero.GetItemById(AbilityId.item_dragon_lance) ?? hero.GetItemById(AbilityId.item_hurricane_pike);
                    if (bonusRangeItem != null)
                    {
                        result += bonusRangeItem.GetAbilitySpecialData("base_attack_range");
                    }
                }

                switch (hero.HeroId)
                {
                    case HeroId.npc_dota_hero_sniper:
                        var sniperTakeAim = hero.GetAbilityById(AbilityId.sniper_take_aim);
                        if (sniperTakeAim?.Level > 0)
                        {
                            result += sniperTakeAim.GetAbilitySpecialData("bonus_attack_range");
                        }

                        break;

                    case HeroId.npc_dota_hero_templar_assassin:
                        var psiBlades = hero.GetAbilityById(AbilityId.templar_assassin_psi_blades);
                        if (psiBlades?.Level > 0)
                        {
                            result += psiBlades.GetAbilitySpecialData("bonus_attack_range");
                        }

                        break;

                    case HeroId.npc_dota_hero_enchantress:
                        var impetus = hero.GetAbilityById(AbilityId.enchantress_impetus);
                        if (impetus?.Level > 0 && hero.HasAghanimsScepter())
                        {
                            result += impetus.GetAbilitySpecialData("bonus_attack_range_scepter");
                        }

                        break;

                    case HeroId.npc_dota_hero_terrorblade:
                        var metamorphosis = hero.GetAbilityById(AbilityId.terrorblade_metamorphosis);
                        if (metamorphosis != null && hero.HasModifier("modifier_terrorblade_metamorphosis"))
                        {
                            var talent = hero.GetAbilityById(AbilityId.special_bonus_unique_terrorblade_3);
                            if (talent?.Level > 0)
                            {
                                result += talent.GetAbilitySpecialData("value");
                            }
                            result += metamorphosis.GetAbilitySpecialData("bonus_range");
                        }

                        break;

                    case HeroId.npc_dota_hero_dragon_knight:
                        var dragonForm = hero.GetAbilityById(AbilityId.dragon_knight_elder_dragon_form);
                        if (dragonForm != null && hero.HasModifier("modifier_dragon_knight_dragon_form"))
                        {
                            result += dragonForm.GetAbilitySpecialData("bonus_attack_range");
                        }

                        break;

                    case HeroId.npc_dota_hero_winter_wyvern:
                        var arcticBurn = hero.GetAbilityById(AbilityId.winter_wyvern_arctic_burn);
                        if (arcticBurn != null && hero.HasModifier("modifier_winter_wyvern_arctic_burn_flight"))
                        {
                            result += arcticBurn.GetAbilitySpecialData("attack_range_bonus");
                        }

                        break;

                    case HeroId.npc_dota_hero_troll_warlord:
                        var trollMeleeForm = hero.GetAbilityById(AbilityId.troll_warlord_berserkers_rage);
                        if (trollMeleeForm != null && hero.HasModifier("modifier_troll_warlord_berserkers_rage"))
                        {
                            result -= trollMeleeForm.GetAbilitySpecialData("bonus_range");
                        }

                        break;

                    case HeroId.npc_dota_hero_lone_druid:
                        var druidMeleeForm = hero.GetAbilityById(AbilityId.lone_druid_true_form);
                        if (druidMeleeForm != null && hero.HasModifier("modifier_lone_druid_true_form"))
                        {
                            // no special data
                            result -= 400;
                        }

                        break;
                    case HeroId.npc_dota_hero_snapfire:
                        var lilShredder = hero.GetAbilityById(AbilityId.snapfire_lil_shredder);
                        if (lilShredder != null && hero.HasModifier("modifier_snapfire_lil_shredder_buff"))
                        {
                            result += lilShredder.GetAbilitySpecialData("attack_range_bonus");
                        }

                        break;
                }
            }

            return result;
        }

        public static float AttackSpeedValue(this Unit unit)
        {
            // TODO are there other modifiers like this one
            if (unit.GetAbilityById(AbilityId.ursa_overpower) != null && unit.HasModifier("modifier_ursa_overpower"))
            {
                return 600;
            }

            if (unit.GetAbilityById(AbilityId.snapfire_lil_shredder) != null && unit.HasModifier("modifier_snapfire_lil_shredder_buff"))
            {
                return 300;
            }

            var attackSpeed = Math.Max(20, unit.AttackSpeed);
            return Math.Min(attackSpeed, 1000);
        }

        public static float CalculateSpellDamage(this Hero source, Unit target, DamageType damageType, float amount)
        {
            switch (damageType)
            {
                case DamageType.Magical:
                    return (1 - target.MagicDamageResist) * (1 + source.GetSpellAmplification()) * amount;
                case DamageType.Physical:
                    return (1 - target.DamageResist) * (1 + source.GetSpellAmplification()) * amount;
                case DamageType.HealthRemoval:
                    return amount;
                case DamageType.Pure:
                    return amount;
            }

            return amount;
        }

        public static bool CanAttack(this Unit unit)
        {
            return unit.AttackCapability != AttackCapability.None && !unit.IsDisarmed();
        }

        public static bool CanAttack(this Unit attacker, Unit target)
        {
            if (target == null || !target.IsValid || !target.IsAlive || !target.IsVisible || !target.IsSpawned || target.IsInvulnerable())
            {
                return false;
            }

            if (attacker.Team == target.Team)
            {
                if (target is Creep)
                {
                    return target.HealthPercent() < 0.5;
                }

                if (target is Hero)
                {
                    return target.HealthPercent() < 0.25;
                }

                if (target is Building)
                {
                    return target.HealthPercent() < 0.10;
                }
            }

            return true;
        }

        public static Vector3 Direction(this Unit unit, float length = 1f)
        {
            var rotation = unit.RotationRad;
            return new Vector3((float)Math.Cos(rotation) * length, (float)Math.Sin(rotation) * length, unit.Position.Z);
        }

        public static Vector2 Direction2D(this Unit unit, float length = 1f)
        {
            var rotation = unit.RotationRad;
            return new Vector2((float)Math.Cos(rotation) * length, (float)Math.Sin(rotation) * length);
        }

        public static float FindRotationAngle(this Unit unit, Vector3 pos)
        {
            var angle = Math.Abs(Math.Atan2(pos.Y - unit.Position.Y, pos.X - unit.Position.X) - unit.RotationRad);

            if (angle > Math.PI)
            {
                angle = Math.Abs((Math.PI * 2) - angle);
            }

            return (float)angle;
        }

        public static Ability GetAbilityById(this Unit unit, AbilityId abilityId)
        {
            return unit.Spellbook.Spells.FirstOrDefault(x => x.Id == abilityId);
        }

        public static float GetAttackDamage(this Unit source, Unit target, bool useMinimumDamage = false, float damageAmplifier = 0.0f)
        {
            float damage = (!useMinimumDamage ? source.DamageAverage : source.MinimumDamage) + source.BonusDamage;
            var mult = 1f;
            var damageType = source.AttackDamageType;
            var armorType = target.ArmorType;

            if (damageType == AttackDamageType.Hero && armorType == ArmorType.Structure)
            {
                mult *= .5f;
            }
            else if (damageType == AttackDamageType.Basic && armorType == ArmorType.Hero)
            {
                mult *= .75f;
            }
            else if (damageType == AttackDamageType.Basic && armorType == ArmorType.Structure)
            {
                mult *= .7f;
            }
            else if (damageType == AttackDamageType.Pierce && armorType == ArmorType.Hero)
            {
                mult *= .5f;
            }
            else if (damageType == AttackDamageType.Pierce && armorType == ArmorType.Basic)
            {
                mult *= 1.5f;
            }
            else if (damageType == AttackDamageType.Pierce && armorType == ArmorType.Structure)
            {
                mult *= .35f;
            }
            else if (damageType == AttackDamageType.Siege && armorType == ArmorType.Hero)
            {
                mult *= .85f;
            }
            else if (damageType == AttackDamageType.Siege && armorType == ArmorType.Structure)
            {
                mult *= 2.50f;
            }

            if (target.IsNeutral || target is Creep && source.IsEnemy(target))
            {
                var isMelee = source.IsMelee;

                var quellingBlade = source.GetItemById(AbilityId.item_quelling_blade);
                if (quellingBlade != null)
                {
                    damage += quellingBlade.GetAbilitySpecialData(isMelee ? "damage_bonus" : "damage_bonus_ranged");
                }

                // apply percentage bonus damage from battle fury to base dmg
                var battleFury = source.GetItemById(AbilityId.item_bfury);
                if (battleFury != null)
                {
                    mult *= battleFury.GetAbilitySpecialData(isMelee ? "quelling_bonus" : "quelling_bonus_ranged") / 100.0f; // 160 | 125
                }
            }

            var armor = target.Armor;

            mult *= 1 - ((0.052f * armor) / (0.9f + (0.048f * Math.Abs(armor))));
            mult *= (1.0f + damageAmplifier);
            return damage * mult;
        }

        public static float GetAutoAttackArrivalTime(this Unit source, Unit target, bool takeRotationTimeIntoAccount = true)
        {
            var result = GetProjectileArrivalTime(source, target, source.AttackPoint(), source.IsMelee ? float.MaxValue : source.ProjectileSpeed(), takeRotationTimeIntoAccount);

            if (!(source is Tower))
            {
                result -= 0.05f; // :broscience:
            }

            return result;
        }

        public static Item GetItemById(this Unit unit, AbilityId abilityId)
        {
            if (!unit.HasInventory)
            {
                return null;
            }

            return unit.Inventory.Items.FirstOrDefault(x => x != null && x.IsValid && x.Id == abilityId);
        }

        public static float GetProjectileArrivalTime(this Unit source, Unit target, float delay, float missileSpeed, bool takeRotationTimeIntoAccount = true)
        {
            var result = 0f;

            // rotation time
            result += takeRotationTimeIntoAccount ? source.TurnTime(target.Position) : 0f;

            // delay
            result += delay;

            // time that takes to the missile to reach the target
            if (missileSpeed != float.MaxValue)
            {
                result += source.Distance2D(target) / missileSpeed;
            }

            return result;
        }

        public static float GetSpellAmplification(this Unit source)
        {
            var spellAmp = 0.0f;

            var hero = source as Hero;
            if (hero != null)
            {
                spellAmp += hero.TotalIntelligence * 0.07f / 100f;
            }

            var kaya = false;
            var yashaAndKaya = false;
            var kayaAndSange = false;

            foreach (var item in source.Inventory.Items)
            {
                switch (item.Id)
                {
                    case AbilityId.item_null_talisman:
                        {
                            spellAmp += item.AbilitySpecialData.First(x => x.Name == "bonus_spell_amp").Value / 100.0f;
                        }
                        break;

                    case AbilityId.item_kaya:
                        {
                            if (kaya)
                            {
                                break;
                            }

                            kaya = true;
                            spellAmp += item.AbilitySpecialData.First(x => x.Name == "spell_amp").Value / 100.0f;
                        }
                        break;

                    case AbilityId.item_yasha_and_kaya:
                        {
                            if (yashaAndKaya)
                            {
                                break;
                            }

                            yashaAndKaya = true;
                            spellAmp += item.AbilitySpecialData.First(x => x.Name == "spell_amp").Value / 100.0f;
                        }
                        break;

                    case AbilityId.item_kaya_and_sange:
                        {
                            if (kayaAndSange)
                            {
                                break;
                            }

                            kayaAndSange = true;
                            spellAmp += item.AbilitySpecialData.First(x => x.Name == "spell_amp").Value / 100.0f;
                        }
                        break;
                }
            }

            var talent = source.Spellbook.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_spell_amplify_"));
            if (talent != null)
            {
                spellAmp += talent.AbilitySpecialData.First(x => x.Name == "value").Value / 100.0f;
            }

            return spellAmp;
        }

        public static bool HasAghanimsScepter(this Unit unit)
        {
            return unit.HasAnyModifiers("modifier_item_ultimate_scepter", "modifier_item_ultimate_scepter_consumed", "modifier_wisp_tether_scepter");
        }

        public static bool HasAnyModifiers(this Unit unit, params string[] modifierNames)
        {
            return unit.Modifiers.Any(x => modifierNames.Contains(x.Name));
        }

        public static bool HasModifier(this Unit unit, string modifierName)
        {
            return unit.Modifiers.Any(modifier => modifier.Name == modifierName);
        }

        public static bool HasModifiers(this Unit unit, IEnumerable<string> modifierNames, bool hasAll = true)
        {
            return hasAll ? modifierNames.All(x => unit.Modifiers.Any(y => y.Name == x)) : unit.Modifiers.Any(x => modifierNames.Contains(x.Name));
        }

        public static float HealthPercent(this Unit unit)
        {
            return (float)unit.Health / unit.MaximumHealth;
        }

        public static Vector3 InFront(this Unit unit, float distance)
        {
            var v = unit.Position + (unit.Vector3FromPolarAngle() * distance);
            return new Vector3(v.X, v.Y, 0);
        }

        public static bool IsAttackImmune(this Unit unit)
        {
            return (unit.UnitState & UnitState.AttackImmune) == UnitState.AttackImmune;
        }

        public static bool IsChanneling(this Unit unit)
        {
            if (unit.HasInventory && unit.Inventory.Items.Any(x => x.IsChanneling))
            {
                return true;
            }

            return unit.Spellbook.Spells.Any(s => s.IsChanneling);
        }

        public static bool IsDirectlyFacing(this Unit source, Vector3 pos)
        {
            var vector1 = pos - source.Position;
            var diff = Math.Abs(Math.Atan2(vector1.Y, vector1.X) - source.RotationRad);
            return diff < 0.025f;
        }

        /// <summary>
        ///     returns true if source is directly facing to target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsDirectlyFacing(this Unit source, Unit target)
        {
            return source.IsDirectlyFacing(target.Position);
        }

        public static bool IsDisarmed(this Unit unit)
        {
            return (unit.UnitState & UnitState.Disarmed) == UnitState.Disarmed;
        }

        public static bool IsEnemy(this Unit unit, Entity target)
        {
            return unit.Team != target.Team;
        }

        /*public static bool IsEthereal(this Unit unit)
        {
            return unit.HasModifiers(EtherealModifiers, false);
        }*/

        public static bool IsInAttackRange(this Unit source, Unit target, float bonusAttackRange = 0.0f)
        {
            return source.IsInRange(target, source.AttackRange(target) + bonusAttackRange, true);
        }

        public static bool IsInRange(this Unit source, Unit target, float range, bool centerToCenter = false)
        {
            return source.Position.IsInRange(target, centerToCenter ? range : Math.Max(0, range + source.HullRadius + target.HullRadius));
        }

        public static bool IsInvisible(this Unit unit)
        {
            return (unit.UnitState & UnitState.Invisible) == UnitState.Invisible;
        }

        public static bool IsInvulnerable(this Unit unit)
        {
            return (unit.UnitState & UnitState.Invulnerable) == UnitState.Invulnerable;
        }

        public static bool IsLinkensProtected(this Unit unit)
        {
            var linkens = unit.GetItemById(AbilityId.item_sphere);
            return linkens?.Cooldown <= 0 || unit.HasModifier("modifier_item_sphere_target");
        }

        public static bool IsSpellShieldProtected(this Unit unit)
        {
            var spellShield = unit.GetAbilityById(AbilityId.antimage_spell_shield);
            return spellShield?.Cooldown <= 0 && unit.HasAghanimsScepter();
        }

        public static bool IsBlockingAbilities(this Unit unit, bool checkReflecting = false)
        {
            if (checkReflecting && unit.HasModifier("modifier_item_lotus_orb_active"))
            {
                return true;
            }

            if (IsLinkensProtected(unit))
            {
                return true;
            }

            if (IsSpellShieldProtected(unit))
            {
                return true;
            }

            // todo qop talent somehow ?

            return false;
        }

        public static bool IsMagicImmune(this Unit unit)
        {
            return (unit.UnitState & UnitState.MagicImmune) == UnitState.MagicImmune;
        }

        public static bool IsMuted(this Unit unit)
        {
            return (unit.UnitState & UnitState.Muted) == UnitState.Muted;
        }

        public static bool IsRealUnit(this Unit unit)
        {
            return unit.UnitType != 0 && (unit.UnitState & UnitState.FakeAlly) != UnitState.FakeAlly;
        }

        public static bool IsBlockingDamage(this Unit unit)
        {
            return unit.HasAnyModifiers(
                "modifier_nyx_assassin_spiked_carapace",
                "modifier_item_combo_breaker_buff",
                "modifier_templar_assassin_refraction_absorb");
        }

        public static bool IsReflectingDamage(this Unit unit)
        {
            return unit.HasAnyModifiers("modifier_nyx_assassin_spiked_carapace", "modifier_item_blade_mail_reflect");
        }

        public static bool IsReflectingAbilities(this Unit unit)
        {
            if (unit.HasModifier("modifier_item_lotus_orb_active"))
            {
                return true;
            }

            var spellShield = unit.GetAbilityById(AbilityId.antimage_spell_shield);
            if (spellShield?.Cooldown <= 0 && unit.HasAghanimsScepter())
            {
                return true;
            }

            return false;
        }

        public static bool IsRooted(this Unit unit)
        {
            return (unit.UnitState & UnitState.Rooted) == UnitState.Rooted;
        }

        public static bool IsRotating(this Unit unit)
        {
            return unit.RotationDifference != 0;
        }

        public static IEnumerable<TEntity> GetUnitsInRange<TEntity>(this Unit unit, float range) where TEntity : Unit, new()
        {
            var handle = unit.Handle;
            var pos = unit.Position;
            var sqrRange = range * range;

            return EntityManager.GetEntities<TEntity>()
                .Where(e => e.Handle != handle && e.IsVisible && e.IsAlive && pos.DistanceSquared(e.Position) < sqrRange)
                .OrderBy(e => pos.DistanceSquared(e.Position));
        }

        public static IEnumerable<TEntity> GetEnemiesInRange<TEntity>(this Unit unit, float range)
            where TEntity : Unit
        {
            var handle = unit.Handle;
            var team = unit.Team;
            var pos = unit.Position;
            var sqrRange = range * range;

            return EntityManager.GetEntities<TEntity>()
                .Where(e => e.Handle != handle && e.IsVisible && e.IsAlive && e.Team != team && pos.DistanceSquared(e.Position) < sqrRange)
                .OrderBy(e => pos.DistanceSquared(e.Position));
        }

        public static bool IsSilenced(this Unit unit)
        {
            return (unit.UnitState & UnitState.Silenced) == UnitState.Silenced;
        }

        public static bool IsStunned(this Unit unit)
        {
            return (unit.UnitState & UnitState.Stunned) == UnitState.Stunned;
        }

        public static bool IsHexed(this Unit unit)
        {
            return (unit.UnitState & UnitState.Hexed) == UnitState.Hexed;
        }

        public static bool IsValidOrbwalkingTarget(this Unit attacker, Unit target, float bonusAttackRange = 0.0f)
        {
            return target.IsValid &&
                   target.IsVisible &&
                   target.IsAlive &&
                   target.IsSpawned &&
                   !target.IsIllusion &&
                   attacker.IsInAttackRange(target, bonusAttackRange) &&
                   !target.IsInvulnerable() &&
                   !target.IsAttackImmune();
        }

        public static bool IsValidTarget(this Unit unit, float range = float.MaxValue, bool checkTeam = true, Vector3? from = null)
        {
            if (unit == null || !unit.IsValid || !unit.IsAlive || !unit.IsVisible || !unit.IsSpawned || unit.IsInvulnerable())
            {
                return false;
            }

            if (checkTeam && unit.Team == EntityManager.LocalHero.Team)
            {
                return false;
            }

            if (range != float.MaxValue)
            {
                return @from == null ? EntityManager.LocalHero.IsInRange(unit, range) : ((Vector3)@from).IsInRange(unit, range);
            }

            return true;
        }

        public static float ProjectileSpeed(this Unit unit)
        {
            try
            {
                return Unit.GetKeyValueByName(unit.Name).GetKeyValue("ProjectileSpeed").GetInt32();
            }
            catch
            {
                LogManager.Warn($"Missing ProjectileSpeed for {unit.Name}");
                return 0;
            }
        }

        public static float TurnRate(this Unit unit, bool currentTurnRate = true)
        {
            float turnRate;
            
            try
            {
                turnRate =  Unit.GetKeyValueByName(unit.Name).GetKeyValue("MovementTurnRate").GetSingle();
            }
            catch
            {
                // Log.Warn($"Missing MovementTurnRate for {unit.Name}");
                turnRate = 0.5f;
            }

            if (currentTurnRate)
            {
                if (unit.HasModifier("modifier_medusa_stone_gaze_slow"))
                {
                    turnRate *= 0.65f;
                }

                if (unit.HasModifier("modifier_batrider_sticky_napalm"))
                {
                    turnRate *= 0.3f;
                }
            }

            return turnRate;
        }

        public static float TurnTime(this Unit unit, Vector3 position)
        {
            return TurnTime(unit, unit.FindRotationAngle(position));
        }

        public static float TurnTime(this Unit unit, Vector2 position)
        {
            return TurnTime(unit, unit.FindRotationAngle(position.ToVector3()));
        }

        public static float TurnTime(this Unit unit, float angle)
        {
            if (angle <= 0.2f)
            {
                return 0;
            }

            return (0.03f / unit.TurnRate()) * angle;
        }

        public static Vector2 Vector2FromPolarAngle(this Unit unit, float delta = 0f, float radial = 1f)
        {
            var diff = MathUtil.DegreesToRadians(unit.RotationDifference);
            var alpha = unit.RotationRad + diff;
            return SharpDXExtensions.FromPolarCoordinates(radial, alpha + delta);
        }

        public static Vector3 Vector3FromPolarAngle(this Unit unit, float delta = 0f, float radial = 1f)
        {
            return Vector2FromPolarAngle(unit, delta, radial).ToVector3();
        }
    }
}