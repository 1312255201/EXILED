// -----------------------------------------------------------------------
// <copyright file="ThrowingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Contains all information before a player throws a grenade.
    /// </summary>
    public class ThrowingItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="request"><inheritdoc cref="RequestType"/></param>
        public ThrowingItemEventArgs(Player player, ThrowableNetworkHandler.RequestType request)
        {
            Player = player;
            Item = player.CurrentItem is Throwable throwable ? throwable : null;
            RequestType = (ThrowRequest)request;
        }

        /// <summary>
        /// Gets the player who's throwing the grenade.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the item being thrown.
        /// </summary>
        public Throwable Item { get; set; }

        /// <summary>
        ///  Gets or sets the type of throw being requested.
        /// </summary>
        public ThrowRequest RequestType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the grenade can be thrown.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}