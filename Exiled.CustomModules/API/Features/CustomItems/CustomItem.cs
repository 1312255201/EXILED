// -----------------------------------------------------------------------
// <copyright file="CustomItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;

    using UnityEngine;

    /// <summary>
    /// A class to easily manage item behavior.
    /// </summary>
    public abstract class CustomItem : TypeCastObject<CustomItem>, IAdditiveBehaviour, IEquatable<CustomItem>, IEquatable<uint>
    {
        /// <inheritdoc cref="ItemManager"/>
        internal static readonly Dictionary<Item, CustomItem> ItemsValue = new();

        /// <inheritdoc cref="PickupManager"/>
        internal static readonly Dictionary<Pickup, CustomItem> PickupValue = new();

        private static readonly List<CustomItem> Registered = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomItem"/>'s.
        /// </summary>
        public static IEnumerable<CustomItem> List => Registered;

        /// <summary>
        /// Gets all Items and their respective <see cref="CustomItem"/>.
        /// </summary>
        public static IReadOnlyDictionary<Item, CustomItem> ItemManager => ItemsValue;

        /// <summary>
        /// Gets all Pickups and their respective <see cref="CustomItem"/>.
        /// </summary>
        public static IReadOnlyDictionary<Pickup, CustomItem> PickupManager => PickupValue;

        /// <summary>
        /// Gets all pickups belonging to a <see cref="CustomItem"/>.
        /// </summary>
        public static HashSet<Pickup> CustomItemsUnhold => PickupManager.Keys.ToHashSet();

        /// <summary>
        /// Gets all items belonging to a <see cref="CustomItem"/>.
        /// </summary>
        public static HashSet<Item> CustomItemsHolded => ItemManager.Keys.ToHashSet();

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s <see cref="Type"/>.
        /// </summary>
        public virtual Type BehaviourComponent { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s description.
        /// </summary>
        public virtual string Description { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomItem"/>'s id.
        /// </summary>
        public virtual uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomItem"/> is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s <see cref="global::ItemType"/>.
        /// </summary>
        public virtual ItemType ItemType { get; }

        /// <summary>
        /// Gets the <see cref="CustomItem"/>'s <see cref="global::ItemCategory"/>.
        /// </summary>
        public virtual ItemCategory ItemCategory { get; }

        /// <summary>
        /// Gets the <see cref="ItemSettings"/>.
        /// </summary>
        public virtual ItemSettings Settings { get; } = ItemSettings.Default;

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomItem"/> is registered.
        /// </summary>
        public virtual bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all pickup owning this <see cref="Pickup"/>.
        /// </summary>
        public IEnumerable<Pickup> Pickups => PickupManager.Where(x => x.Value.Id == Id).Select(x => x.Key);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Item"/> containing all item owning this <see cref="CustomItem"/>.
        /// </summary>
        public IEnumerable<Item> Items => ItemsValue.Where(x => x.Value.Id == Id).Select(x => x.Key);

        /// <summary>
        /// Compares two operands: <see cref="CustomItem"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomItem"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomItem left, object right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomItem"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator ==(object left, CustomItem right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="CustomItem"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomItem"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomItem left, object right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="left">The left <see cref="object"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomItem"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomItem right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="CustomItem"/> and <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomItem"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomItem"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomItem left, CustomItem right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Compares two operands: <see cref="CustomItem"/> and <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomItem"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomItem"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomItem left, CustomItem right) => !(left.Id == right.Id);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> given the specified <paramref name="customItemType"/>.
        /// </summary>
        /// <param name="customItemType">The specified <see cref="Id"/>.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomItem Get(object customItemType) => Registered.FirstOrDefault(customItem => customItem == customItemType && customItem.IsEnabled);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomItem Get(string name) => Registered.FirstOrDefault(customItem => customItem.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomItem Get(Type type) => type.BaseType != typeof(ItemBehaviour) ? null : Registered.FirstOrDefault(customItem => customItem.BehaviourComponent == type);

        /// <summary>
        /// Gets a <see cref="CustomItem"/> given the specified <see cref="ItemBehaviour"/>.
        /// </summary>
        /// <param name="itemBuilder">The specified <see cref="ItemBehaviour"/>.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomItem Get(ItemBehaviour itemBuilder) => Get(itemBuilder.GetType());

        /// <summary>
        /// Gets a <see cref="CustomItem"/> from a <see cref="Item"/>.
        /// </summary>
        /// <param name="item">The <see cref="CustomItem"/> owner.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomItem Get(Item item)
        {
            CustomItem customItem = default;

            foreach (KeyValuePair<Item, CustomItem> kvp in ItemManager)
            {
                if (kvp.Key != item)
                    continue;

                customItem = Get(kvp.Value.Id);
            }

            return customItem;
        }

        /// <summary>
        /// Gets a <see cref="CustomItem"/> from a <see cref="Pickup"/>.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> owner.</param>
        /// <returns>The <see cref="CustomItem"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomItem Get(Pickup pickup)
        {
            CustomItem customItem = default;

            foreach (KeyValuePair<Pickup, CustomItem> kvp in PickupManager)
            {
                if (kvp.Key != pickup)
                    continue;

                customItem = Get(kvp.Value.Id);
            }

            return customItem;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> given the specified <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="customItemType">The <see cref="object"/> to look for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(object customItemType, out CustomItem customItem) => customItem = Get(customItemType);

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomItem"/> name to look for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomItem customItem) => customItem = Registered.FirstOrDefault(cItem => cItem.Name == name);

        /// <summary>
        /// Tries to get the item's current <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to search on.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Item item, out CustomItem customItem) => customItem = Get(item);

        /// <summary>
        /// Tries to get the item's current <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> to search on.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pickup pickup, out CustomItem customItem) => customItem = Get(pickup);

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> given the specified <see cref="ItemBehaviour"/>.
        /// </summary>
        /// <param name="itemBuilder">The <see cref="ItemBehaviour"/> to search for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(ItemBehaviour itemBuilder, out CustomItem customItem) => customItem = Get(itemBuilder.GetType());

        /// <summary>
        /// Tries to get a <see cref="CustomItem"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to search for.</param>
        /// <param name="customItem">The found <see cref="CustomItem"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomItem customItem) => customItem = Get(type.GetType());

        /// <summary>
        /// Attempts to spawn the specified custom item.
        /// </summary>
        /// <param name="postion">The location where the custom item spawn.</param>
        /// <param name="customItem">The custom item.</param>
        /// <param name="pickup">The <see cref="Pickup"/> instance of the <see cref="CustomItem"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the custeom item successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method attempts to spawn the specified player with the given custom item. If the custom item is not provided
        /// or is invalid, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool TrySpawn(Vector3 postion, CustomItem customItem, out Pickup pickup)
        {
            pickup = default;

            if (!customItem)
                return false;

            pickup = customItem.Spawn(postion);

            return true;
        }

        /// <summary>
        /// Attempts to spawn the specified custom item item identified by the provided type or type name.
        /// </summary>
        /// <param name="postion">The location where the custom item spawn.</param>
        /// <param name="customItemType">The type or type name of the custom item.</param>
        /// <param name="pickup">The <see cref="Pickup"/> instance of the <see cref="CustomItem"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the custom item was spawned successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows attempting to spawn the specified custom item identified by its type or type name.
        /// If the custom item type or name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool TrySpawn(Vector3 postion, object customItemType, out Pickup pickup)
        {
            pickup = default;

            if (!TryGet(customItemType, out CustomItem customItem))
                return false;

            TrySpawn(postion, customItem, out pickup);

            return true;
        }

        /// <summary>
        /// Attempts to spawn the specified custom item by the provided name.
        /// </summary>
        /// <param name="postion">The location where the custom item spawn.</param>
        /// <param name="name">The name of the custom item.</param>
        /// <param name="pickup">The <see cref="Pickup"/> instance of the <see cref="CustomItem"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the custom item was spawned successfully; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method allows attempting to spawn the specified custom item identified by its name.
        /// If the custom item name is not provided, or if the identification process fails, the method returns <see langword="false"/>.
        /// </remarks>
        public static bool TrySpawn(Vector3 postion, string name, out Pickup pickup)
        {
            pickup = default;

            if (!TryGet(name, out CustomItem customItem))
                return false;

            TrySpawn(postion, customItem, out pickup);

            return true;
        }

        /// <summary>
        /// Gives to a specific <see cref="Player"/> a specic <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="name">The name of the <see cref="CustomItem"/> to give.</param>
        /// <param name="displayMessage">Indicates a value whether <see cref="ItemSettings.PickedUpMessage"/> will be called when the player receives the <see cref="CustomItem"/> or not.</param>
        /// <returns>Returns a value indicating if the player was given the <see cref="CustomItem"/> or not.</returns>
        public static bool TryGive(Player player, string name, bool displayMessage = true)
        {
            if (!TryGet(name, out CustomItem item))
                return false;

            item?.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Gives to a specific <see cref="Player"/> a specic <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="id">The IDs of the <see cref="CustomItem"/> to give.</param>
        /// <param name="displayMessage">Indicates a value whether <see cref="ItemSettings.PickedUpMessage"/> will be called when the player receives the <see cref="CustomItem"/> or not.</param>
        /// <returns>Returns a value indicating if the player was given the <see cref="CustomItem"/> or not.</returns>
        public static bool TryGive(Player player, uint id, bool displayMessage = true)
        {
            if (!TryGet(id, out CustomItem item))
                return false;

            item?.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Gives to a specific <see cref="Player"/> a specic <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        /// <param name="t">The <see cref="System.Type"/> of the item to give.</param>
        /// <param name="displayMessage">Indicates a value whether <see cref="ItemSettings.PickedUpMessage"/> will be called when the player receives the <see cref="CustomItem"/> or not.</param>
        /// <returns>Returns a value indicating if the player was given the <see cref="CustomItem"/> or not.</returns>
        public static bool TryGive(Player player, Type t, bool displayMessage = true)
        {
            if (!TryGet(t, out CustomItem item))
                return false;

            item?.Give(player, displayMessage);

            return true;
        }

        /// <summary>
        /// Enables all the custom items present in the assembly.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="CustomItem"/> containing all the enabled custom items.
        /// </returns>
        /// <remarks>
        /// This method dynamically enables all custom items found in the calling assembly. Custom items
        /// must be marked with the <see cref="CustomItemAttribute"/> to be considered for enabling. If
        /// a custom item is enabled successfully, it is added to the returned list.
        /// </remarks>
        public static List<CustomItem> EnableAll()
        {
            List<CustomItem> customItems = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                CustomItemAttribute attribute = type.GetCustomAttribute<CustomItemAttribute>();
                if ((type.BaseType != typeof(CustomItem) && !type.IsSubclassOf(typeof(CustomItem))) || attribute is null)
                    continue;

                CustomItem customItem = Activator.CreateInstance(type) as CustomItem;

                if (!customItem.IsEnabled)
                    continue;

                if (customItem.TryRegister(attribute))
                    customItems.Add(customItem);
            }

            if (customItems.Count != Registered.Count())
                Log.Info($"{customItems.Count} custom items have been successfully registered!");

            return customItems;
        }

        /// <summary>
        /// Disables all the custom items present in the assembly.
        /// </summary>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="CustomItem"/> containing all the disabled custom items.
        /// </returns>
        /// <remarks>
        /// This method dynamically disables all custom items found in the calling assembly that were
        /// previously registered. If a custom item is disabled successfully, it is added to the returned list.
        /// </remarks>
        public static List<CustomItem> DisableAll()
        {
            List<CustomItem> customItems = new();
            customItems.AddRange(Registered.Where(customItem => customItem.TryUnregister()));

            Log.Info($"{customItems.Count} custom items have been successfully unregistered!");

            return customItems;
        }

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(float x, float y, float z) => Spawn(new Vector3(x, y, z));

        /// <summary>
        /// Spawns a <see cref="ItemType"/> as a <see cref="CustomItem"/> in a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="item">The <see cref="ItemType"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(float x, float y, float z, Item item) => Spawn(new Vector3(x, y, z), item);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> where a specific <see cref="Player"/> is, and optionally sets the previous owner.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="previousOwner">The previous owner of the pickup, can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Player player, Player previousOwner = null) => Spawn(player.Position, previousOwner);

        /// <summary>
        /// Spawns a <see cref="ItemType"/> as a <see cref="CustomItem"/> where a specific <see cref="Player"/> is, and optionally sets the previous owner.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> position where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="ItemType"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <param name="previousOwner">The previous owner of the pickup, can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Player player, Item item, Player previousOwner = null) => Spawn(player.Position, item, previousOwner);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="previousOwner">The <see cref="Pickup.PreviousOwner"/> of the item. Can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Vector3 position, Player previousOwner = null) => Spawn(position, Item.Create(ItemType), previousOwner);

        /// <summary>
        /// Spawns the <see cref="CustomItem"/> in a specific position.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> where the <see cref="CustomItem"/> will be spawned.</param>
        /// <param name="item">The <see cref="ItemType"/> to be spawned as a <see cref="CustomItem"/>.</param>
        /// <param name="previousOwner">The <see cref="Pickup.PreviousOwner"/> of the item. Can be null.</param>
        /// <returns>The <see cref="Pickup"/> of the spawned <see cref="CustomItem"/>.</returns>
        public virtual Pickup Spawn(Vector3 position, Item item, Player previousOwner = null)
        {
            Pickup pickup = item.CreatePickup(position);
            pickup.Scale = Settings.Scale;
            if (Settings.Weight != -1)
                pickup.Weight = Settings.Weight;

            if (previousOwner is not null)
                pickup.PreviousOwner = previousOwner;

            PickupValue.Add(pickup, this);

            return pickup;
        }

        /// <summary>
        /// Gives an <see cref="ItemType"/> as a <see cref="CustomItem"/> to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="item">The <see cref="ItemType"/> to be given.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ItemSettings.PickedUpMessage"/> will be called when the player receives the item.</param>
        public virtual void Give(Player player, Item item, bool displayMessage = true)
        {
            try
            {
                Log.Debug($"{Name}.{nameof(Give)}: Item Serial: {item.Serial} Ammo: {(item is Firearm firearm ? firearm.Ammo : -1)}");

                player.AddItem(item);

                Log.Debug($"{nameof(Give)}: Adding {item.Serial} to tracker.");

                ItemsValue.Add(item, this);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(Give)}: {e}");
            }
        }

        /// <summary>
        /// Gives a <see cref="Pickup"/> as a <see cref="CustomItem"/> to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="pickup">The <see cref="Pickup"/> to be given.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ItemSettings.PickedUpMessage"/> will be called when the player receives the item.</param>
        public virtual void Give(Player player, Pickup pickup, bool displayMessage = true) => Give(player, player.AddItem(pickup), displayMessage);

        /// <summary>
        /// Gives the <see cref="CustomItem"/> to a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who will receive the item.</param>
        /// <param name="displayMessage">Indicates whether or not <see cref="ItemSettings.PickedUpMessage"/> will be called when the player receives the item.</param>
        public virtual void Give(Player player, bool displayMessage = true) => Give(player, Item.Create(ItemType), displayMessage);

        /// <summary>
        /// Determines whether id is equal to the current object.
        /// </summary>
        /// <param name="id">The id to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(uint id)
        {
            return Id == id;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="cr">The custom role to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomItem cr)
        {
            if (cr is null)
            {
                return false;
            }

            if (ReferenceEquals(this, cr))
            {
                return true;
            }

            return Id == cr.Id;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (Equals(obj as CustomItem))
                return true;

            try
            {
                return Equals((uint)obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Tries to register a <see cref="CustomItem"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomItemAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomItem"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomItemAttribute attribute = null)
        {
            if (!Registered.Contains(this))
            {
                if (attribute is not null && Id == 0)
                    Id = attribute.Id;

                if (Registered.Any(x => x.Id == Id))
                {
                    Log.Warn(
                        $"Couldn't register {Name}. " +
                        $"Another custom item has been registered with the same id:" +
                        $" {Registered.FirstOrDefault(x => x.Id == Id)}");

                    return false;
                }

                Registered.Add(this);

                return true;
            }

            Log.Warn($"Couldn't register {Name}. This custom item has been already registered.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomItem"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomItem"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug($"Couldn't unregister {Name}. This custom item hasn't been registered yet.");

                return false;
            }

            Registered.Remove(this);

            return true;
        }
    }
}
