// -----------------------------------------------------------------------
// <copyright file="IndividualFriendlyFire.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
#pragma warning disable SA1402
#pragma warning disable SA1649
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using Footprinting;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Checks friendly fire rules.
    /// </summary>
    public static class IndividualFriendlyFire
    {
        /// <summary>
        /// Checks if there can be damage between two players, according to the FF rules.
        /// </summary>
        /// <param name="attackerHub">The person attacking.</param>
        /// <param name="victimHub">The person being attacked.</param>
        /// <param name="attackerRole">The attackers current role.</param>
        /// <returns>True if the attacker can damage the victim.</returns>
        public static bool CheckFriendlyFirePlayerFriendly(ReferenceHub attackerHub, ReferenceHub victimHub, RoleType attackerRole)
        {
            if (Server.FriendlyFire)
                return true;
            if (attackerHub == null || victimHub == null)
                return true;
            Player attacker = Player.Get(attackerHub);
            Player victim = Player.Get(victimHub);
            if (attacker == null || victim == null)
                return true;

            return attacker.IsFriendlyFireEnabled || victim.Side != attackerRole.GetSide();
        }
    }

    /// <summary>
    /// Patches <see cref="HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool) })]
    internal static class HitboxIdentityCheckFriendlyFire
    {
        private static bool Prefix(ReferenceHub attacker, ReferenceHub victim, bool ignoreConfig, ref bool __result)
        {
            try
            {
                __result = IndividualFriendlyFire.CheckFriendlyFirePlayerFriendly(attacker,  victim, attacker == null ? RoleType.None : attacker.characterClassManager.CurClass);

                return false;
            }
            catch (Exception e)
            {
                Log.Error($"{e}");
                return true;
            }
        }
    }

    /// <summary>
    /// Patches <see cref="HitboxIdentity.Damage(float, InventorySystem.Items.IDamageDealer, Footprinting.Footprint, UnityEngine.Vector3)"/>.
    /// </summary>
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.Damage))]
    internal static class HitboxIdentityDamagePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int instructionsToRemove = 10;
            int offset = -1;
            int index = newInstructions.FindIndex(code => code.opcode == OpCodes.Ldfld && (FieldInfo)code.operand == Field(typeof(Role), nameof(Role.roleId))) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, false)
            newInstructions.InsertRange(index, new[]
            {
                // AttackerFootprint.Hub
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // this.TargetHub
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(HitboxIdentity), nameof(HitboxIdentity.TargetHub))),

                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Role))),
                new CodeInstruction(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerFriendly))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ExplosionGrenade.ExplodeDestructible(IDestructible)"/>.
    /// </summary>
    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.ExplodeDestructible))]
    internal static class ExplosionGrenadeExplodeDestructiblePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int targetIsOwnerIndex = 5;
            const int offset = 6;
            const int instructionsToRemove = 8;

            int index = newInstructions.FindIndex(code => code.opcode == OpCodes.Stloc_S &&
                ((LocalBuilder)code.operand).LocalIndex == targetIsOwnerIndex) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, false)
            newInstructions.InsertRange(index, new[]
            {
                // this.PreviousOwner.Hub
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // targetReferenceHub
                new CodeInstruction(OpCodes.Ldloc_3),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Role))),
                new CodeInstruction(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerFriendly))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.PlayExplosionEffects()"/>.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
    internal static class FlashbangGrenadePlayExplosionEffectsPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;
            const int instructionsToRemove = 9;
            int index = newInstructions.FindLastIndex(code => code.opcode == OpCodes.Brtrue_S) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, false)
            newInstructions.InsertRange(index, new[]
            {
                // this.PreviousOwner.Hub
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // KeyValuePair<GameObject, ReferenceHub>.Value (target ReferenceHub)
                new CodeInstruction(OpCodes.Ldloca_S, 2),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Value))),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Role))),
                new CodeInstruction(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerFriendly))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp018Projectile.DetectPlayers()"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp018Projectile), nameof(Scp018Projectile.DetectPlayers))]
    internal static class Scp018ProjectileDetectPlayersPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int referenceHubIndex = 1;
            const int offset = 5;
            const int instructionsToRemove = 7;

            int index = newInstructions.FindLastIndex(code => code.opcode == OpCodes.Ldloca_S &&
                ((LocalBuilder)code.operand).LocalIndex == referenceHubIndex) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // HitboxIdentity.CheckFriendlyFire(ReferenceHub, ReferenceHub, false)
            newInstructions.InsertRange(index, new[]
            {
                // Scp018Projectile.PreviousOwner.Hub
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(Scp018Projectile), nameof(Scp018Projectile.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),

                // targetReferenceHub
                new CodeInstruction(OpCodes.Ldloc_1),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(Scp018Projectile), nameof(Scp018Projectile.PreviousOwner))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Role))),
                new CodeInstruction(OpCodes.Call, Method(typeof(IndividualFriendlyFire), nameof(IndividualFriendlyFire.CheckFriendlyFirePlayerFriendly))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
