// -----------------------------------------------------------------------
// <copyright file="Landing.cs" company="Exiled Team">
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
    /// Patches <see cref="FootstepSync.RpcPlayLandingFootstep(bool)"/>
    /// Adds the <see cref="Player.Landing"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FootstepSync))]
    internal static class Landing
    {
        [HarmonyPatch(nameof(FootstepSync.RpcPlayLandingFootstep))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> LandingTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FootstepSync), nameof(FootstepSync._ccm))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(LandingEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnLanding))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
