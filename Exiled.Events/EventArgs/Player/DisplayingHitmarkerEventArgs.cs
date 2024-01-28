﻿// -----------------------------------------------------------------------
// <copyright file="DisplayingHitmarkerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before displaying the hitmarker to the player.
    /// </summary>
    public class DisplayingHitmarkerEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayingHitmarkerEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="size"><inheritdoc cref="Size"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DisplayingHitmarkerEventArgs(Player player, float size, bool isAllowed = true)
        {
            Player = player;
            Size = size;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the hitmarker's size.
        /// </summary>
        public float Size { get; set; }
    }
}