// -----------------------------------------------------------------------
// <copyright file="ActivatingGeneratorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using MapGeneration.Distributors;

    /// <summary>
    /// Contains all information before a player inserts a tablet into a generator.
    /// </summary>
    public class ActivatingGeneratorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingGeneratorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="generator"><inheritdoc cref="Generator"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ActivatingGeneratorEventArgs(Player player, Scp079Generator generator, bool isAllowed = true)
        {
            Player = player;
            Generator = generator;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's inserting a tablet into the generator.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="Scp079Generator"/> instance.
        /// </summary>
        public Scp079Generator Generator { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the tablet can be inserted.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
