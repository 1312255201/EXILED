// -----------------------------------------------------------------------
// <copyright file="Scp939Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayableScps;

    /// <summary>
    /// Defines a role that represents SCP-939.
    /// </summary>
    public class Scp939Role : Role
    {
        private Scp939 script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp939Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        /// <param name="scp939Type">The type of SCP-939.</param>
        internal Scp939Role(Player player, RoleType scp939Type)
        {
            Owner = player;
            Type = scp939Type;
            script = player.ReferenceHub.scpsController.CurrentScp as Scp939;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <inheritdoc/>
        public override RoleType Type { get; }

        /// <summary>
        /// Gets or sets the amount of time before SCP-939 can attack again.
        /// </summary>
        public float AttackCooldown
        {
            get => script.CurrentBiteCooldown;
            set => script.CurrentBiteCooldown = value;
        }

        /// <summary>
        /// Gets SCP-939's move speed.
        /// </summary>
        public float MoveSpeed => script.GetMovementSpeed();
    }
}
