// -----------------------------------------------------------------------
// <copyright file="UsingScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244Item"/> to add missing event handler to the <see cref="Scp244Item.ServerOnUsingCompleted"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp244Item), nameof(Scp244Item.ServerOnUsingCompleted))]
    internal static class UsingScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();

            int index = 0;
#pragma warning disable SA1118 // Parameter should not span multiple lines

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Ldarg_0),

                new(OpCodes.Ldarg_0),

                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp244Item), nameof(Scp244Item.Owner))),

                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingScp244EventArgs))[0]),

                new(OpCodes.Dup),

                new(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnUsingScp244))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingScp244EventArgs), nameof(UsingScp244EventArgs.IsAllowed))),

                new(OpCodes.Brtrue_S, continueProcessing),

                new CodeInstruction(OpCodes.Ret).WithLabels(ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
