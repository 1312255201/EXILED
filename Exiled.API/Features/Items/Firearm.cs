// -----------------------------------------------------------------------
// <copyright file="Firearm.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CameraShaking;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Structs;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.Firearms.Firearm"/>.
    /// </summary>
    public class Firearm : Item
    {
        /// <summary>
        /// Gets a <see cref="IReadOnlyDictionary{TKey, TValue}"/> which contains all pairs for <see cref="ItemType"/> and <see cref="Enums.BaseCode"/>.
        /// </summary>
        internal static readonly IReadOnlyDictionary<ItemType, BaseCode> FirearmPairs = new Dictionary<ItemType, BaseCode>()
        {
            { ItemType.GunCOM15, BaseCode.GunCOM15 },
            { ItemType.GunCOM18, BaseCode.GunCOM18 },
            { ItemType.GunRevolver, BaseCode.GunRevolver },
            { ItemType.GunE11SR, BaseCode.GunE11SR },
            { ItemType.GunCrossvec, BaseCode.GunCrossvec },
            { ItemType.GunFSP9, BaseCode.GunFSP9 },
            { ItemType.GunLogicer, BaseCode.GunLogicer },
            { ItemType.GunAK, BaseCode.GunAK },
            { ItemType.GunShotgun, BaseCode.GunShotgun },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="InventorySystem.Items.Firearms.Firearm"/> class.</param>
        public Firearm(InventorySystem.Items.Firearms.Firearm itemBase)
            : base(itemBase)
        {
            Base = itemBase;

            switch (Base)
            {
                case AutomaticFirearm auto:
                    Base.AmmoManagerModule =
                        new AutomaticAmmoManager(auto, auto._baseMaxAmmo, 1, auto._boltTravelTime == 0);
                    break;
                case Shotgun shotgun:
                    Base.AmmoManagerModule = new TubularMagazineAmmoManager(shotgun, Serial, shotgun._ammoCapacity, shotgun._numberOfChambers, 0.5f, 3, "ShellsToLoad", ActionName.Zoom, ActionName.Shoot);
                    break;
                default:
                    Base.AmmoManagerModule = new ClipLoadedInternalMagAmmoManager(Base, 6);
                    break;
            }

            Base._status = new FirearmStatus(MaxAmmo, FirearmStatusFlags.MagazineInserted, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the firearm.</param>
        internal Firearm(ItemType type)
            : this((InventorySystem.Items.Firearms.Firearm)Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }

        /// <inheritdoc cref="AvailableAttachmentsValue"/>.
        public static IReadOnlyDictionary<ItemType, AttachmentIdentifier[]> AvailableAttachments => AvailableAttachmentsValue;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> which represents all the preferences for each <see cref="Player"/>.
        /// </summary>
        public static IReadOnlyDictionary<Player, Dictionary<ItemType, AttachmentIdentifier[]>> PlayerPreferences
        {
            get
            {
                IEnumerable<KeyValuePair<Player, Dictionary<ItemType, AttachmentIdentifier[]>>> playerPreferences = AttachmentsServerHandler.PlayerPreferences.Select((KeyValuePair<ReferenceHub, Dictionary<ItemType, uint>> keyValuePair) =>
                {
                    return new KeyValuePair<Player, Dictionary<ItemType, AttachmentIdentifier[]>>(Player.Get(keyValuePair.Key), keyValuePair.Value.ToDictionary(kvp => kvp.Key, kvp => kvp.Key.GetAttachmentIdentifiers(kvp.Value).ToArray()));
                });

                return playerPreferences.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.Firearms.Firearm"/> that this class is encapsulating.
        /// </summary>
        public new InventorySystem.Items.Firearms.Firearm Base { get; }

        /// <summary>
        /// Gets or sets the amount of ammo in the firearm.
        /// </summary>
        public byte Ammo
        {
            get => Base.Status.Ammo;
            set => Base.Status = new FirearmStatus(value, Base.Status.Flags, Base.Status.Attachments);
        }

        /// <summary>
        /// Gets the max ammo for this firearm.
        /// </summary>
        public byte MaxAmmo => Base.AmmoManagerModule.MaxAmmo;

        /// <summary>
        /// Gets the <see cref="Enums.AmmoType"/> of the firearm.
        /// </summary>
        public AmmoType AmmoType => Base.AmmoType.GetAmmoType();

        /// <summary>
        /// Gets a value indicating whether the firearm is being aimed.
        /// </summary>
        public bool Aiming => Base.AdsModule.ServerAds;

        /// <summary>
        /// Gets a value indicating whether the firearm's flashlight module is enabled.
        /// </summary>
        public bool FlashlightEnabled => Base.Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled);

        /// <summary>
        /// Gets the <see cref="FirearmAttachment"/>s of the firearm.
        /// </summary>
        public FirearmAttachment[] Attachments => Base.Attachments;

        /// <summary>
        /// Gets the <see cref="AttachmentIdentifier"/>s of the firearm.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> AttachmentIdentifiers => this.GetAttachmentIdentifiers();

        /// <summary>
        /// Gets the <see cref="Enums.BaseCode"/> of the firearm.
        /// </summary>
        public BaseCode BaseCode => FirearmPairs[Type];

        /// <summary>
        /// Gets or sets the fire rate of the firearm, if it is an automatic weapon.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set this value for a weapon that is semi-automatic.</exception>
        public float FireRate
        {
            get => Base is AutomaticFirearm auto ? auto._fireRate : 1f;
            set
            {
                if (Base is AutomaticFirearm auto)
                    auto._fireRate = value;
                else
                    throw new InvalidOperationException("You cannot change the firerate of non-automatic weapons.");
            }
        }

        /// <summary>
        /// Gets or sets the recoil settings of the firearm, if it's an automatic weapon.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set this value for a weapon that is semi-automatic.</exception>
        public RecoilSettings Recoil
        {
            get => Base is AutomaticFirearm auto ? auto._recoil : default;
            set
            {
                if (Base is AutomaticFirearm auto)
                    auto.ActionModule = new AutomaticAction(Base, auto._semiAutomatic, auto._boltTravelTime, 1f / auto._fireRate, auto._dryfireClipId, auto._triggerClipId, auto._gunshotPitchRandomization, value, auto._recoilPattern, false);
                else
                    throw new InvalidOperationException("You cannot change the recoil pattern of non-automatic weapons.");
            }
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="AttachmentIdentifier"/>[] which contains all available attachments for all firearms.
        /// </summary>
        internal static Dictionary<ItemType, AttachmentIdentifier[]> AvailableAttachmentsValue { get; } = new Dictionary<ItemType, AttachmentIdentifier[]>();

        /// <summary>
        /// Adds a <see cref="AttachmentIdentifier"/> to the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to add.</param>
        public void AddAttachment(AttachmentIdentifier identifier)
        {
            uint toRemove = 0;
            uint code = 1;
            foreach (FirearmAttachment attachment in Base.Attachments)
            {
                if (attachment.Name == identifier.Name && attachment.IsEnabled)
                {
                    toRemove = code;
                    break;
                }

                code *= 2;
            }

            uint newCode = identifier.Code == 0 ?
                AvailableAttachments[Type].FirstOrDefault(attId =>
                attId.Name == identifier.Name).Code :
                identifier.Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~toRemove | newCode, true);
        }

        /// <summary>
        /// Adds a <see cref="FirearmAttachment"/> of the specified <see cref="AttachmentNameTranslation"/> to the firearm.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentNameTranslation"/> to add.</param>
        public void AddAttachment(AttachmentNameTranslation attachmentName) => AddAttachment(new AttachmentIdentifier(attachmentName));

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to the firearm.
        /// </summary>
        /// <param name="identifiers">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to add.</param>
        public void AddAttachment(IEnumerable<AttachmentIdentifier> identifiers)
        {
            foreach (AttachmentIdentifier identifier in identifiers)
                AddAttachment(identifier);
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="AttachmentNameTranslation"/> to the firearm.
        /// </summary>
        /// <param name="attachmentNames">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentNameTranslation"/> to add.</param>
        public void AddAttachment(IEnumerable<AttachmentNameTranslation> attachmentNames)
        {
            foreach (AttachmentNameTranslation attachmentName in attachmentNames)
                AddAttachment(new AttachmentIdentifier(attachmentName));
        }

        /// <summary>
        /// Removes a <see cref="AttachmentIdentifier"/> from the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to remove.</param>
        public void RemoveAttachment(AttachmentIdentifier identifier)
        {
            if (!Attachments.Any(attachment => attachment.Name == identifier.Name && attachment.IsEnabled))
                return;

            uint code = identifier.Code == 0 ?
                AvailableAttachments[Type].FirstOrDefault(attId =>
                attId.Name == identifier.Name).Code :
                identifier.Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);
        }

        /// <summary>
        /// Removes a <see cref="FirearmAttachment"/> of the specified <see cref="AttachmentNameTranslation"/> from the firearm.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentNameTranslation"/> to remove.</param>
        public void RemoveAttachment(AttachmentNameTranslation attachmentName)
        {
            FirearmAttachment firearmAttachment = Attachments.FirstOrDefault(att => att.Name == attachmentName && att.IsEnabled);

            if (firearmAttachment is null)
                return;

            uint code = AvailableAttachments[Type].FirstOrDefault(attId => attId == firearmAttachment).Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);
        }

        /// <summary>
        /// Removes a <see cref="FirearmAttachment"/> of the specified <see cref="AttachmentSlot"/> from the firearm.
        /// </summary>
        /// <param name="attachmentSlot">The <see cref="AttachmentSlot"/> to remove.</param>
        public void RemoveAttachment(AttachmentSlot attachmentSlot)
        {
            FirearmAttachment firearmAttachment = Attachments.FirstOrDefault(att => att.Slot == attachmentSlot && att.IsEnabled);

            if (firearmAttachment is null)
                return;

            uint code = AvailableAttachments[Type].FirstOrDefault(attId => attId == firearmAttachment).Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);
        }

        /// <summary>
        /// Removes a <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> from the firearm.
        /// </summary>
        /// <param name="identifiers">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentIdentifier> identifiers)
        {
            foreach (AttachmentIdentifier identifier in identifiers)
                RemoveAttachment(identifier);
        }

        /// <summary>
        /// Removes a list of <see cref="FirearmAttachment"/> of the specified <see cref="IEnumerable{T}"/> of <see cref="AttachmentNameTranslation"/> from the firearm.
        /// </summary>
        /// <param name="attachmentNames">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentNameTranslation"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentNameTranslation> attachmentNames)
        {
            foreach (AttachmentNameTranslation attachmentName in attachmentNames)
                RemoveAttachment(attachmentName);
        }

        /// <summary>
        /// Removes a list of <see cref="FirearmAttachment"/> of the specified <see cref="IEnumerable{T}"/> of <see cref="AttachmentSlot"/> from the firearm.
        /// </summary>
        /// <param name="attachmentSlots">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentSlot"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentSlot> attachmentSlots)
        {
            foreach (AttachmentSlot attachmentSlot in attachmentSlots)
                RemoveAttachment(attachmentSlot);
        }

        /// <summary>
        /// Removes all attachments from the firearm.
        /// </summary>
        public void ClearAttachments() => Base.ApplyAttachmentsCode((uint)BaseCode, true);

        /// <summary>
        /// Gets a <see cref="FirearmAttachment"/> of the specified <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to check.</param>
        /// <returns>The corresponding <see cref="FirearmAttachment"/>.</returns>
        public FirearmAttachment GetAttachment(AttachmentIdentifier identifier) => Attachments.FirstOrDefault(attachment => attachment == identifier);

        /// <summary>
        /// Tries to get a <see cref="FirearmAttachment"/> of the specified <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to check.</param>
        /// <param name="firearmAttachment">The corresponding <see cref="FirearmAttachment"/>.</param>
        /// <returns>A value indicating whether or not the firearm has the specified <see cref="FirearmAttachment"/>.</returns>
        public bool TryGetAttachment(AttachmentIdentifier identifier, out FirearmAttachment firearmAttachment)
        {
            firearmAttachment = default;

            if (!Attachments.Any(attachment => attachment.Name == identifier.Name))
                return false;

            firearmAttachment = GetAttachment(identifier);

            return true;
        }

        /// <summary>
        /// Tries to get a <see cref="FirearmAttachment"/> of the specified <see cref="AttachmentNameTranslation"/>.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentNameTranslation"/> to check.</param>
        /// <param name="firearmAttachment">The corresponding <see cref="FirearmAttachment"/>.</param>
        /// <returns>A value indicating whether or not the firearm has the specified <see cref="FirearmAttachment"/>.</returns>
        public bool TryGetAttachment(AttachmentNameTranslation attachmentName, out FirearmAttachment firearmAttachment)
        {
            firearmAttachment = default;

            if (!Attachments.Any(attachment => attachment.Name == attachmentName))
                return false;

            firearmAttachment = GetAttachment(new AttachmentIdentifier(attachmentName));

            return true;
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be added.</param>
        /// <param name="itemType">The <see cref="ItemType"/> to add.</param>
        /// <param name="attachments">The <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(Player player, ItemType itemType, AttachmentIdentifier[] attachments)
        {
            foreach (KeyValuePair<Player, Dictionary<ItemType, AttachmentIdentifier[]>> kvp in PlayerPreferences)
            {
                if (kvp.Key != player)
                    continue;

                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player.ReferenceHub, out Dictionary<ItemType, uint> dictionary))
                    dictionary[itemType] = attachments.GetAttachmentsCode();
            }
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="KeyValuePair{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(Player player, KeyValuePair<ItemType, AttachmentIdentifier[]> preference) => AddPreference(player, preference.Key, preference.Value);

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(Player player, Dictionary<ItemType, AttachmentIdentifier[]> preference)
        {
            foreach (KeyValuePair<ItemType, AttachmentIdentifier[]> kvp in preference)
                AddPreference(player, kvp);
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be added.</param>
        /// <param name="itemType">The <see cref="ItemType"/> to add.</param>
        /// <param name="attachments">The <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(IEnumerable<Player> players, ItemType itemType, AttachmentIdentifier[] attachments)
        {
            foreach (Player player in players)
                AddPreference(player, itemType, attachments);
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="KeyValuePair{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(IEnumerable<Player> players, KeyValuePair<ItemType, AttachmentIdentifier[]> preference)
        {
            foreach (Player player in players)
                AddPreference(player, preference.Key, preference.Value);
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(IEnumerable<Player> players, Dictionary<ItemType, AttachmentIdentifier[]> preference)
        {
            foreach ((Player player, KeyValuePair<ItemType, AttachmentIdentifier[]> kvp) in players.SelectMany(player => preference.Select(kvp => (player, kvp))))
                AddPreference(player, kvp);
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be removed.</param>
        /// <param name="itemType">The <see cref="ItemType"/> to remove.</param>
        public void RemovePreference(Player player, ItemType itemType)
        {
            foreach (KeyValuePair<Player, Dictionary<ItemType, AttachmentIdentifier[]>> kvp in PlayerPreferences)
            {
                if (kvp.Key != player)
                    continue;

                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player.ReferenceHub, out Dictionary<ItemType, uint> dictionary))
                    dictionary[itemType] = (uint)itemType.GetBaseCode();
            }
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be removed.</param>
        /// <param name="itemType">The <see cref="ItemType"/> to remove.</param>
        public void RemovePreference(IEnumerable<Player> players, ItemType itemType)
        {
            foreach (Player player in players)
                RemovePreference(player, itemType);
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be removed.</param>
        /// <param name="itemTypes">The <see cref="IEnumerable{T}"/> of <see cref="ItemType"/> to remove.</param>
        public void RemovePreference(Player player, IEnumerable<ItemType> itemTypes)
        {
            foreach (ItemType itemType in itemTypes)
                RemovePreference(player, itemType);
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be removed.</param>
        /// <param name="itemTypes">The <see cref="IEnumerable{T}"/> of <see cref="ItemType"/> to remove.</param>
        public void RemovePreference(IEnumerable<Player> players, IEnumerable<ItemType> itemTypes)
        {
            foreach ((Player player, ItemType itemType) in players.SelectMany(player => itemTypes.Select(itemType => (player, itemType))))
                RemovePreference(player, itemType);
        }

        /// <summary>
        /// Clears all the existing preferences from <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be cleared.</param>
        public void ClearPreferences(Player player)
        {
            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player.ReferenceHub, out Dictionary<ItemType, uint> dictionary))
            {
                foreach (KeyValuePair<ItemType, uint> kvp in dictionary)
                    dictionary[kvp.Key] = (uint)kvp.Key.GetBaseCode();
            }
        }

        /// <summary>
        /// Clears all the existing preferences from <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be cleared.</param>
        public void ClearPreferences(IEnumerable<Player> players)
        {
            foreach (Player player in players)
                ClearPreferences(player);
        }

        /// <summary>
        /// Clears all the existing preferences from <see cref="PlayerPreferences"/>.
        /// </summary>
        public void ClearPreferences()
        {
            foreach (Player player in Player.List)
                ClearPreferences(player);
        }
    }
}
