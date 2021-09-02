// -----------------------------------------------------------------------
// <copyright file="ProcessingHotkeyEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before pressing a hotkey.
    /// </summary>
    public class ProcessingHotkeyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingHotkeyEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="hotkey"><inheritdoc cref="Hotkey"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ProcessingHotkeyEventArgs(Player player, HotkeyButton hotkey, bool isAllowed = true)
        {
            Player = player;
            Hotkey = hotkey;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's pressing the hotkey.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the pressed hotkey.
        /// </summary>
        public HotkeyButton Hotkey { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the hotkey is allowed to be pressed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
