// -----------------------------------------------------------------------
// <copyright file="ChangingAttachments.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;

using InventorySystem.Items;

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
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

            const int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAttachmentsEventArgs));
            LocalBuilder curCode = generator.DeclareLocal(typeof(uint));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // curCode = Firearm::GetCurrentAttachmentsCode
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Stloc_S, curCode.LocalIndex),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // If the Firearm::GetCurrentAttachmentsCode isn't changed, prevents the method from being executed
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                new CodeInstruction(OpCodes.Ldloc_S, curCode.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, ret),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // API::Features::Player::Get(NetworkConnection::identity::netId)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // Item::Get(firearm)
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Items.Item), nameof(API.Features.Items.Item.Get))),
                new CodeInstruction(OpCodes.Castclass, typeof(API.Features.Items.Firearm)),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // AttachmentsChangeRequest::AttachmentsCode
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // ChangingAttachmentsEventArgs ev = new ChangingAttachmentsEventArgs(__ARGS__)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAttachmentsEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // Handlers::Item::OnChangingAttachments(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingAttachments))),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // ev.IsAllowed
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),

                // **AttachmentsChangeRequest = ev::NewCode + curCode - ev::CurrentCode
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.NewCode))),
                new CodeInstruction(OpCodes.Ldloc_S, curCode.LocalIndex),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.CurrentCode))),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),

                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingAttachments), nameof(InvokeDebug))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void InvokeDebug() => Log.Debug($"{DateTime.Now}.{DateTime.Now.Millisecond}");
    }
}
