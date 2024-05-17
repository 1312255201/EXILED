// -----------------------------------------------------------------------
// <copyright file="TeleportList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using API.Features;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="RoomManager.GenerateMap(int)"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoomManager), nameof(RoomManager.GenerateMap))]
    internal class TeleportList
    {
        private static void Postfix()
        {
            Map.TeleportsValue.Clear();
            Map.TeleportsValue.AddRange(UnityEngine.Object.FindObjectsOfType<PocketDimensionTeleport>());
        }
    }
}