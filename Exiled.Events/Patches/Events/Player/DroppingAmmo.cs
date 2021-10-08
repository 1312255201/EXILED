// -----------------------------------------------------------------------
// <copyright file="DroppingAmmo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventorySystem.Inventory.UserCode_CmdDropItem"/>.
    /// Adds the <see cref="DroppingAmmo"/> event.
    /// </summary>
    [HarmonyPatch(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory.UserCode_CmdDropAmmo))]
    internal static class DroppingAmmo
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var offset = -6;
            var index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            var ev = generator.DeclareLocal(typeof(DroppingAmmoEventArgs));
            var returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ReferenceHub);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // ammoType
                new CodeInstruction(OpCodes.Ldarg_1),

                // amount
                new CodeInstruction(OpCodes.Ldarg_2),

                // var ev = DroppingAmmoEventArgs(...)
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingAmmoEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Player.OnDroppingAmmo(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDroppingAmmo))),

                // if (!ev.IsAllowed) return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DroppingAmmoEventArgs), nameof(DroppingAmmoEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
