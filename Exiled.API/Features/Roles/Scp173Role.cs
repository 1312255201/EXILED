// -----------------------------------------------------------------------
// <copyright file="Scp173Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a role that represents SCP-173.
    /// </summary>
    public class Scp173Role : Role
    {
        private PlayableScps.Scp173 script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp173Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp173Role(Player player)
        {
            Owner = player;
            script = player.ReferenceHub.scpsController.CurrentScp as PlayableScps.Scp173;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <inheritdoc/>
        public override RoleType Type => RoleType.Scp173;

        /// <summary>
        /// Gets a value indicating whether or not SCP-173 is currently being viewed by one or more players.
        /// </summary>
        public bool IsObserved => script._isObserved;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of players that are currently viewing SCP-173. Can be empty.
        /// </summary>
        public IReadOnlyCollection<Player> ObservingPlayers
        {
            get => script._observingPlayers.Select(hub => Player.Get(hub)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets SCP-173's move speed.
        /// </summary>
        public float MoveSpeed => script.GetMoveSpeed();

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173 is able to blink.
        /// </summary>
        public bool BlinkReady
        {
            get => script.BlinkReady;
            set => script.BlinkReady = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can blink.
        /// </summary>
        public float BlinkCooldown
        {
            get => script._blinkCooldownRemaining;
            set = script._blinkCooldownRemaining = value;
        }

        /// <summary>
        /// Gets a value indicating the max distance that SCP-173 can move in a blink. Factors in <see cref="BreakneckActive"/>.
        /// </summary>
        public float BlinkDistance => script.EffectiveBlinkDistance();

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173's breakneck speed is active.
        /// </summary>
        public bool BreakneckActive
        {
            get => script.BreakneckSpeedsActive;
            set => script.BreakneckSpeedsActive = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can use breackneck speed again.
        /// </summary>
        public float BreakneckCooldown
        {
            get => script._breakneckSpeedsCooldownRemaining;
            set => script._breakneckSpeedsCooldownRemaining = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can place a tantrum.
        /// </summary>
        public float TantrumCooldown
        {
            get => script._tantrumCooldownRemaining;
            set => script._tantrumCooldownRemaining = value;
        }

        /// <summary>
        /// Force places a tantrum.
        /// </summary>
        /// <param name="failIfObserved">Whether or not to place the tantrum if SCP-173 is currently being viewed.</param>
        public void Tantrum(bool failIfObserved = false)
        {
            if (failIfObserved && IsObserved)
                return;

            Owner.PlaceTantrum();
        }
    }
}
