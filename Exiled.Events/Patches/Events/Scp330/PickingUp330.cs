// -----------------------------------------------------------------------
// <copyright file="PickingUp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Interactables.Interobjects;

    using InventorySystem.Items.Usables.Scp330;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp330Bag.ServerProcessPickup"/> method to add the <see cref="Handlers.Player.PickingUpScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup))]
    internal static class PickingUp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            LocalBuilder ev = generator.DeclareLocal(typeof(PickingUpScp330EventArgs));
            Label continueLabel = generator.DefineLabel();

            // We put the event code right at the beginning of the method.
            newInstructions.InsertRange(0, new[]
            {
                // var ev = new PickingUpSco330EventArgs(Player.Get(ply), pickup);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpScp330EventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnPickingUpScp330(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPickingUp330))),

                // if (!ev.IsAllowed)
                //    return false;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpScp330EventArgs), nameof(PickingUpScp330EventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue, continueLabel),

                // We need to load false onto the stack before returning, since the method returns a bool.
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
