// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static Exiled.Events.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]
    internal static class Scp173BeingLooked
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = -3;

            // Search for the only "HashSet<ReferenceHub>.Add()".
            int index = newInstructions.FindLastIndex(instruction =>
                instruction.opcode == OpCodes.Callvirt && (MethodInfo)instruction.operand ==
                Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Add))) + offset;

            // Declare Player, to be able to store its instance with "stloc.2".
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            // Define the continue label and add it.
            Label continueLabel = generator.DefineLabel();
            newInstructions[index + 5].labels.Add(continueLabel);

            // Player player = Player.Get(gameObject);
            //
            // if (player == null || (player.Role == RoleType.Tutorial && Exiled.Events.Events.Instance.Config.CanTutorialBlockScp173)
            //   continue;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, continueLabel),
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (int)RoleType.Tutorial),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue, continueLabel),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Instance))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Config), nameof(Config.CanTutorialBlockScp173))),
                new CodeInstruction(OpCodes.Brfalse, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
