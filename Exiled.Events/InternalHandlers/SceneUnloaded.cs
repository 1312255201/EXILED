// -----------------------------------------------------------------------
// <copyright file="SceneUnloaded.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.InternalHandlers
{
    using UnityEngine.SceneManagement;

#pragma warning disable SA1611 // Element parameters should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Handles scene unload event.
    /// </summary>
    internal static class SceneUnloaded
    {
        /// <summary>
        /// Called once when the server changes the scene.
        /// </summary>
        /// <remarks>
        /// This fixes the main issue with ghost mode,
        /// when it spams with a NRE error.
        /// Before that, we were clearing the cache
        /// on WaitForPlayers event, but
        /// sometimes (ordinally on silent rount restart)
        /// the server accepts players' tokens before
        /// WaitForPlayers event is called.
        /// </remarks>
        public static void OnSceneUnloaded(Scene _)
        {
            API.Features.Player.IdsCache.Clear();
            API.Features.Player.UserIdsCache.Clear();
            API.Features.Player.Dictionary.Clear();
        }
    }
}
