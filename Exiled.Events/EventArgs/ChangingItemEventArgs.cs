// -----------------------------------------------------------------------
// <copyright file="ChangingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items;

    /// <summary>
    /// Contains all information before a player's held item changes.
    /// </summary>
    public class ChangingItemEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newItem"><inheritdoc cref="NewItem"/></param>
        public ChangingItemEventArgs(Player player, ItemBase newItem)
        {
            Player = player;
            NewItem = Item.Get(newItem);
        }

        /// <summary>
        /// Gets the player who's changing the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the new item.
        /// </summary>
        public Item NewItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event is allowed to continue.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
