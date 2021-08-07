// -----------------------------------------------------------------------
// <copyright file="GainingExperience.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1118
#pragma warning disable SA1123
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.UserCode_RpcGainExp(ExpGainType, RoleType)"/>.
    /// Adds the <see cref="Handlers.Scp079.GainingExperience"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.UserCode_RpcGainExp))]
    internal static class GainingExperience
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Declare a local variable of the type "GainingExperienceEventArgs".
            var gainingExperienceEv = generator.DeclareLocal(typeof(GainingExperienceEventArgs));

            // Define the continue label.
            var continueLabel = generator.DefineLabel();

            // Define the return label.
            var returnLabel = generator.DefineLabel();

            // var ev = new GainingExperienceEventArgs(Player.Get(this.gameObject), type, (float)details, true)
            newInstructions.InsertRange(0, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // type
                new CodeInstruction(OpCodes.Ldarg_1),

                // (float)details
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Conv_R4),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new GainingExperienceEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(GainingExperienceEventArgs))[0]),
                new CodeInstruction(OpCodes.Stloc_S, gainingExperienceEv.LocalIndex),
            });

            #region ExpGainType.KillAssist and ExpGainType.PocketAssist

            // The index offset.
            var offset = 0;

            // Search for the first "call NetworkServer.active".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == PropertyGetter(typeof(NetworkServer), nameof(NetworkServer.active))) + offset;

            // ev.Amount = num2
            // goto continueLabel
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, gainingExperienceEv.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.Amount))),
                new CodeInstruction(OpCodes.Br_S, continueLabel),
            });

            #endregion

            #region ExpGainType.GeneralInteractions

            // The index offset.
            offset = 1;

            // Search for the last "stloc.3".
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Stloc_3) + offset;

            // ev.Amount = num3
            // goto continueLabel
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, gainingExperienceEv.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.Amount))),
                new CodeInstruction(OpCodes.Br_S, continueLabel),
            });

            #endregion

            #region ExpGainType.AdminCheat

            // The index offset.
            offset = 0;

            // Search for the last "call NetworkServer.active".
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == PropertyGetter(typeof(NetworkServer), nameof(NetworkServer.active))) + offset;

            // goto continueLabel
            newInstructions.Insert(index, new CodeInstruction(OpCodes.Br_S, continueLabel));

            #endregion

            // Handlers.Scp079.OnGainingExperience(ev);
            //
            // if (!ev.IsAllowed || ev.Amount <= 0)
            //   return;
            //
            // this.AddExperience(ev.Amount);
            newInstructions.AddRange(new[]
            {
                // Handlers.Scp079.OnGainingExperience(ev);
                new CodeInstruction(OpCodes.Ldloc_S, gainingExperienceEv.LocalIndex).WithLabels(continueLabel).MoveLabelsFrom(newInstructions[newInstructions.Count - 1]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnGainingExperience))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),

                // if (ev.Amount <= 0)
                //   return;
                new CodeInstruction(OpCodes.Ldloc_S, gainingExperienceEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.Amount))),
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Ble_Un_S, returnLabel),

                // this.AddExperience(ev.Amount);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, gainingExperienceEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GainingExperienceEventArgs), nameof(GainingExperienceEventArgs.Amount))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.AddExperience))),
                new CodeInstruction(OpCodes.Ret).WithLabels(returnLabel),
            });

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
