// -----------------------------------------------------------------------
// <copyright file="ChangingAttachments.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Firearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Patches <see cref="AttachmentsServerHandler.ServerReceiveChangeRequest(NetworkConnection, AttachmentsChangeRequest)"/>.
    /// Adds the <see cref="Handlers.Item.ChangingAttachments"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceiveChangeRequest))]
    internal static class ChangingAttachments
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;

            // Values to keep track of both the original codes and the results obtained from IL calculations
            LocalBuilder wCode_0x01 = generator.DeclareLocal(typeof(uint));

            // The event to implement
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAttachmentsEventArgs));

            // Value to extend results obtained from iterations involving memory addresses
            LocalBuilder store_data_0x01 = generator.DeclareLocal(typeof(List<AttachmentIdentifier>));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // If the Firearm::GetCurrentAttachmentsCode isn't changed, prevents the method from being executed
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, ret),

                // API::Features::Player::Get(NetworkConnection::identity::netId)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),

                // firearm
                new CodeInstruction(OpCodes.Ldloc_1),

                // GetAttachmentIdentifiers(firearm::ItemTypeId, firearm::GetCurrentAttachmentsCode)
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.ItemTypeId))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentIdentifiers), new[] { typeof(ItemType), typeof(uint) })),

                // firearm::ItemTypeId::GetAttachmentIdentifiers(AttachmentsChangeRequest::AttachmentsCode)
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.ItemTypeId))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentIdentifiers), new[] { typeof(ItemType), typeof(uint) })),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // ChangingAttachmentsEventArgs ev = new ChangingAttachmentsEventArgs(__ARGS__)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAttachmentsEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers::Item::OnChangingAttachments(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingAttachments))),

                // ev.IsAllowed
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),

                // store_data_0x01 = *ev.NewAttachmentIdentifiers
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.NewAttachmentIdentifiers))),
                new CodeInstruction(OpCodes.Stloc_S, store_data_0x01.LocalIndex),

                // wCode_0x01 = *store_data_0x01::GetAttachmentsCode + **firearm::GetCurrentAttachmentsCode - **identifiers_0x02::GetAttachmentsCode
                new CodeInstruction(OpCodes.Ldloc_S, store_data_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentsCode))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.OldAttachmentIdentifiers))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentsCode))),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x01.LocalIndex),

                // **AttachmentsChangeRequest = wCode_0x01
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void InvokeDebug(string debug) => API.Features.Log.Debug(debug);
    }
}
