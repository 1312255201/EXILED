// -----------------------------------------------------------------------
// <copyright file="Usable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Usables;

    /// <summary>
    /// A wrapper class for <see cref="UsableItem"/>.
    /// </summary>
    public class Usable : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Usable"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Usable(ItemBase itemBase)
            : base(itemBase)
        {
            Base = (UsableItem)itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Usable"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Item.Type"/></param>
        public Usable(ItemType type)
            : base(type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return;

            Base = (UsableItem)itemBase;
        }

        /// <inheritdoc cref="Item.Base"/>
        public new UsableItem Base { get; }

        /// <summary>
        /// Gets a value indicating whether this item is equippable.
        /// </summary>
        public bool Equippable => Base.AllowEquip;

        /// <summary>
        /// Gets a value indicating whether this item is holsterable.
        /// </summary>
        public bool Holsterable => Base.AllowHolster;

        /// <summary>
        /// Gets or sets the Weight of the item.
        /// </summary>
        public float Weight
        {
            get => Base.Weight;
            set => Base._weight = value;
        }

        /// <summary>
        /// Gets a value indicating whether the item is currently being used.
        /// </summary>
        public bool IsUsing => Base.IsUsing;

        /// <summary>
        /// Gets or sets how long it takes to use the item.
        /// </summary>
        public float UseTime
        {
            get => Base.UseTime;
            set => Base.UseTime = value;
        }

        /// <summary>
        /// Gets or sets how long after using starts a player has to cancel using the item.
        /// </summary>
        public float MaxCancellableTime
        {
            get => Base.MaxCancellableTime;
            set => Base.MaxCancellableTime = value;
        }

        /// <summary>
        /// Gets or sets the cooldown between repeated uses of this item.
        /// </summary>
        public float RemainingCooldown
        {
            get => Base.RemainingCooldown;
            set => Base.RemainingCooldown = value;
        }
    }
}
