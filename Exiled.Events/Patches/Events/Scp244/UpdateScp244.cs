// -----------------------------------------------------------------------
// <copyright file="UpdateScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup.UpdateRange"/>.
    /// Adds the <see cref="Handlers.Scp244.OpeningScp244"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.UpdateRange))]
    internal static class UpdateScp244
    {
        private static bool Prefix(Scp244DeployablePickup __instance)
        {
            try
            {
                if (__instance.ModelDestroyed && __instance._visibleModel.activeSelf)
                {
                    __instance.Rb.constraints = RigidbodyConstraints.FreezeAll;
                    __instance._visibleModel.SetActive(false);
                }

                if (!NetworkServer.active)
                {
                    __instance.CurrentSizePercent = __instance._syncSizePercent;
                    __instance.CurrentSizePercent /= 255f;
                    return false;
                }

                if (__instance.State == Scp244State.Idle)
                {
                    OpeningScp244EventArgs ev = new(__instance, Vector3.Dot(__instance.transform.up, Vector3.up) < __instance._activationDot);
                    Handlers.Scp244.OnOpeningScp244(ev);
                    if (ev.IsAllowed)
                    {
                        __instance.State = Scp244State.Active;
                        __instance._lifeTime.Restart();
                    }
                }

            int continueOffset = 0;
            int continueIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State)))) + continueOffset;

#pragma warning disable SA1118 // Parameter should not span multiple lines
            newInstructions.InsertRange(index, new[]
            {
                // Load arg 0 (No param, instance of object) EStack[Scp244DeployablePickup Instance]
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load the field within the instance, since get; set; we can use PropertyGetter to get state. EStack[State]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State))),

                // What to compare again (Idle State) EStack[State, 0]
                new CodeInstruction(OpCodes.Ldc_I4_0),

                // If they are not equal, we do not do our logic and we skip nw logic EStack[]
                new CodeInstruction(OpCodes.Bne_Un, continueProcessing),

                // Used for instance call EStack[Scp244DeployablePickup Instance] --------------------
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load the field within the instance, since no get; set; we can use Field. EStack[transform]
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.transform))),

                // Load the field within the instance, since no get; set; we can use Field. EStack[Transform.up]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UnityEngine.Transform), nameof(Transform.up))),

                // Load the field within the instance, since no get; set; we can use Field. EStack[Transform.up, Vector3.up]
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Vector3), nameof(Vector3.up))),

                // Load the field within the instance, since no get; set; we can use Field. EStack[Vector3.Dot]
                new CodeInstruction(OpCodes.Call, Method(typeof(Vector3), nameof(Vector3.Dot), new[] { typeof(Vector3), typeof(Vector3) })),

                // Second parameter EStack[Vector3.Dot, Scp244DeployablePickup Instance] -----------
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load our activation dot EStack[Vector3.Dot, _activationDot]
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._activationDot))),

                // Verify if dot product < activation EStack[result]
                new CodeInstruction(OpCodes.Clt),

                // If the dot product is less than we jump, otherwise continue EStack[]
                new CodeInstruction(OpCodes.Brfalse, continueProcessing),

                // Load the Scp244DeployablePickup instance EStack[Scp244DeployablePickup Instance]
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load our true since it is a bool anyway, EStack[Scp244DeployablePickup Instance, 1]
                new CodeInstruction(OpCodes.Ldc_I4_1, continueProcessing),

                // Pass all 2 variables to OpeningScp244EventArgs New Object, get a new object in return EStack[OpeningScp244EventArgs Instance]
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningScp244EventArgs))[0]),

                // Copy it for later use again EStack[OpeningScp244EventArgs Instance, PickingUpScp244EventArgs Instance]
                new CodeInstruction(OpCodes.Dup),

                // Call Method on Instance EStack[OpeningScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnOpeningScp244))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpScp244EventArgs), nameof(PickingUpScp244EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, false return occurs below EStack[]
                new CodeInstruction(OpCodes.Brfalse, continueProcessing),

                // Load the Scp244DeployablePickup instance EStack[Scp244DeployablePickup Instance] ------
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load with Scp244State.Active EStack[Scp244DeployablePickup Instance, Active]
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // Load the field within the instance, since get; set; we can use PropertySetter to set state. EStack[]
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State))),

                // Load the Scp244DeployablePickup instance EStack[Scp244DeployablePickup Instance]
                new CodeInstruction(OpCodes.Ldarg_0),

                // Load the field within the instance, since get; set; we can use PropertyGetter to get state. EStack[_lifeTime]
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._lifeTime))),

                // Load the field within the instance, since get; set; we can use PropertyGetter to get state. EStack[]
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Stopwatch), nameof(Stopwatch.Restart))),

                // We finished our if logic, now we will continue on with normal logic
                new CodeInstruction(OpCodes.Br, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Nop).WithLabels(returnFalse),
                new CodeInstruction(OpCodes.Ret),

                // Good route of is allowed being true 
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            // Jumping over original NW logic. 
            newInstructions.InsertRange(continueIndex, new[]
            {
                new CodeInstruction(OpCodes.Nop).WithLabels(normalProcessing),
            });

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(UsingScp244).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
