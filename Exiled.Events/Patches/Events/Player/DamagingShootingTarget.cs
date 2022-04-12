// -----------------------------------------------------------------------
// <copyright file="DamagingShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using AdminToys;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ShootingTarget.Damage(float, DamageHandlerBase, Vector3)"/>.
    /// Adds the <see cref="Handlers.Player.DamagingShootingTarget"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.Damage))]
    internal static class DamagingShootingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(DamagingShootingTargetEventArgs));

            Label jccLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker))),
                new(OpCodes.Ldfld, Field(typeof(Footprinting.Footprint), nameof(Footprinting.Footprint.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingShootingTargetEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDamagingShootingTarget))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingShootingTargetEventArgs), nameof(DamagingShootingTargetEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, jccLabel),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jccLabel),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingShootingTargetEventArgs), nameof(DamagingShootingTargetEventArgs.Amount))),
                new(OpCodes.Starg_S, 1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingShootingTargetEventArgs), nameof(DamagingShootingTargetEventArgs.Amount))),
                new(OpCodes.Stloc_2),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
