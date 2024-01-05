// -----------------------------------------------------------------------
// <copyright file="RemovingAbilityEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomAbilities
{
    using Exiled.API.Features.Core;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all informations before removing an ability.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="GameEntity"/>.</typeparam>
    public class RemovingAbilityEventArgs<T> : IDeniableEvent
        where T : GameEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingAbilityEventArgs{T}"/> class.
        /// </summary>
        /// <param name="entity"><inheritdoc cref="Entity"/></param>
        /// <param name="ability"><inheritdoc cref="Ability"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public RemovingAbilityEventArgs(T entity, CustomAbility<T> ability, bool isAllowed = true)
        {
            Entity = entity;
            Ability = ability;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the custom ability being removed.
        /// </summary>
        public CustomAbility<T> Ability { get; }

        /// <summary>
        /// Gets the entity to which the custom ability is being removed.
        /// </summary>
        public T Entity { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}