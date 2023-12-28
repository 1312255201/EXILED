// -----------------------------------------------------------------------
// <copyright file="ActiveAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using System;

    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;

    using MEC;

    /// <summary>
    /// Represents the base class for active ability behaviors associated with a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity associated with the ability behavior.</typeparam>
    public abstract class ActiveAbilityBehaviour<TEntity> : AbilityBehaviourBase<TEntity>
        where TEntity : GameEntity
    {
        private bool isDurationBased;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before a
        /// <typeparamref name="TEntity"/> <see cref="GameEntity"/> activates the ability.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnActivatingDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after a
        /// <typeparamref name="TEntity"/> <see cref="GameEntity"/> activates the ability.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnActivatedDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired after the ability expires.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<IAbilityBehaviour> OnExpiredDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the last time the ability was used.
        /// </summary>
        public DateTime LastUsed { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the ability is ready.
        /// </summary>
        public virtual bool IsReady => DateTime.Now > LastUsed + TimeSpan.FromSeconds(Settings.Cooldown);

        /// <summary>
        /// Forces the cooldown making the ability usable.
        /// </summary>
        /// <returns>The new <see cref="LastUsed"/> value, representing the updated cooldown timestamp.</returns>
        public virtual DateTime ForceCooldown() => LastUsed = DateTime.Now - TimeSpan.FromSeconds(Settings.Cooldown + 1);

        /// <summary>
        /// Resets the cooldown making the ability not usable.
        /// </summary>
        /// <param name="forceExpiration">A value indicating whether the ability should expire, if active.</param>
        /// <returns>The new <see cref="LastUsed"/> value, representing the updated cooldown timestamp.</returns>
        public virtual DateTime ResetCooldown(bool forceExpiration = false)
        {
            if (forceExpiration)
                IsActive = false;

            return LastUsed = DateTime.Now;
        }

        /// <inheritdoc/>
        public override void AdjustAddittivePipe()
        {
            LastUsed = Settings.ForceCooldownOnAdded ? ForceCooldown() : ResetCooldown();
            isDurationBased = Settings.Duration > 0f;
        }

        /// <summary>
        /// Activates the ability.
        /// </summary>
        /// <param name="isForced">A value indicating whether the activation should be forced.</param>
        /// <returns><see langword="true"/> if the ability was activated; otherwise, <see langword="false"/>.</returns>
        public virtual bool Activate(bool isForced = false)
        {
            if (isForced || IsReady)
            {
                OnActivating();
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            SubscribeEvents_Static();
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (IsActive)
                OnUsing();
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            UnsubscribeEvents_Static();
        }

        /// <summary>
        /// Fired before the ability is activated.
        /// </summary>
        protected virtual void OnActivating()
        {
            OnActivatingDispatcher.InvokeAll(this);

            Timing.CallDelayed(Settings.WindupTime, OnActivated);
        }

        /// <summary>
        /// Fired after the ability is activated.
        /// </summary>
        protected virtual void OnActivated()
        {
            Execute();

            if (isDurationBased)
            {
                IsActive = true;

                SubscribeEvents();
                Timing.CallDelayed(Settings.Duration, OnExpired);
            }

            OnActivatedDispatcher.InvokeAll(this);
        }

        /// <summary>
        /// Fired when the ability time's up.
        /// <para>This method will be fired using duration-based abilities.</para>
        /// </summary>
        protected virtual void OnExpired()
        {
            IsActive = false;

            UnsubscribeEvents();
            ResetCooldown();

            OnExpiredDispatcher.InvokeAll(this);
        }

        /// <summary>
        /// Executes the ability once, or permanently if behaves passive.
        /// <para>This method will be fired as soon as the ability is activated.</para>
        /// </summary>
        protected abstract void Execute();
    }
}