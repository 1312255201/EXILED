﻿// -----------------------------------------------------------------------
// <copyright file="VoiceChatting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using API.Features.Roles;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.Voice;

    using VoiceChat.Networking;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// patches <see cref="VoiceTransceiver.ServerReceiveMessage(NetworkConnection, VoiceMessage)"/> to add the <see cref="Handlers.Player.VoiceChatting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    internal static class VoiceChatting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            Label voiceRoleLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(VoiceChattingEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder voiceRole = generator.DeclareLocal(typeof(IVoiceRole));

            newInstructions[0].labels.Add(voiceRoleLabel);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(msg.Speaker);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // if (player.Role.Base is not IVoiceRole voiceRole)
                //      ignore.
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Base))),
                new(OpCodes.Isinst, typeof(IVoiceRole)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, voiceRole.LocalIndex),
                new(OpCodes.Ldnull),
                new(OpCodes.Cgt_Un),
                new(OpCodes.Brfalse_S, voiceRoleLabel),

                // Player.Get(msg.Speaker);
                new(OpCodes.Ldloc_S, player.LocalIndex),

                // msg
                new(OpCodes.Ldarg_1),

                // voiceModule.
                new(OpCodes.Ldloc_S, voiceRole.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(IVoiceRole), nameof(IVoiceRole.VoiceModule))),

                // true
                new(OpCodes.Ldc_I4_1),

                // VoiceChattingEventArgs ev = new(Player, VoiceMessage);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(VoiceChattingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnVoiceChatting(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnVoiceChatting))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceChattingEventArgs), nameof(VoiceChattingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                // ev.VoiceMessage = msg;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(VoiceChattingEventArgs), nameof(VoiceChattingEventArgs.VoiceMessage))),
                new(OpCodes.Stloc_1),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}