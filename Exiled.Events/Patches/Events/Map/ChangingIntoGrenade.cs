// -----------------------------------------------------------------------
// <copyright file="ChangingIntoGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Item = Exiled.API.Features.Items.Item;

    /// <summary>
    /// Patches <see cref="InventorySystem.Items.ThrowableProjectiles.TimedGrenadePickup.Update"/>.
    /// Adds the <see cref="Handlers.Map.ChangingIntoGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(InventorySystem.Items.ThrowableProjectiles.TimedGrenadePickup), nameof(InventorySystem.Items.ThrowableProjectiles.TimedGrenadePickup.Update))]
    internal static class ChangingIntoGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset
            int offset = 1;

            // Find the last return false call.
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            // Extract the existing label we will be removing.
            Label enterLabel = newInstructions[index].labels[0];

            // Generate a return label.
            Label returnLabel = generator.DefineLabel();

            // Generate a label for when we cannot set the fuse time.
            Label skipFuse = generator.DefineLabel();

            // Declare ChangingIntoGrenadeEventArgs, to be able to store it's instance with "stloc.s".
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingIntoGrenadeEventArgs));

            // Declare TimeGrenade, so we are able to set it's fusetime.
            LocalBuilder timeGrenade = generator.DeclareLocal(typeof(TimeGrenade));

            // Declare ThrownProjectile because the base method doesn't use it as a local.
            LocalBuilder thrownProjectile = generator.DeclareLocal(typeof(ThrownProjectile));

            // Remove the existing instructions that get the itemBase to spawn, we will be doing this ourselves.
            int instructionsToRemove = 14;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // Setup EventArgs, call event, check ev.IsAllowed and implement ev.Type changing
            newInstructions.InsertRange(index, new[]
            {
                // itemPickupBase
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(enterLabel),

                // var ev = new ChangingIntoGrenadeEventArgs(ItemPickupBase);
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingIntoGrenadeEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Map.OnChangingIntoGrenade(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnChangingIntoGrenade))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // if (!InventoryItemLoader.AvailableItems.TryGetValue(ev.Type, out itemBase) || !(itemBase is ThrowableItem throwableItem))
                //    return;
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(InventoryItemLoader), nameof(InventoryItemLoader.AvailableItems))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.Type))),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, ItemBase>), nameof(Dictionary<ItemType, ItemBase>.TryGetValue))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Isinst, typeof(ThrowableItem)),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            offset = 4;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_1) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // if (thrownProjectile is TimeGrenade timeGrenade)
                //    timeGrenade._fuseTime = ev.FuseTime;
                new CodeInstruction(OpCodes.Stloc, thrownProjectile.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, thrownProjectile.LocalIndex),
                new CodeInstruction(OpCodes.Isinst, typeof(TimeGrenade)),
                new CodeInstruction(OpCodes.Stloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, skipFuse),

                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.FuseTime))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new CodeInstruction(OpCodes.Nop).WithLabels(skipFuse),
                new CodeInstruction(OpCodes.Ldloc, thrownProjectile.LocalIndex),
                new CodeInstruction(OpCodes.Dup),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
