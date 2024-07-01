// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomRoles
{
    using System;

    /// <inheritdoc/>
    /// <typeparam name="T">The <see cref="RoleBehaviour"/> type.</typeparam>
    public abstract class CustomRole<T> : CustomRole
        where T : RoleBehaviour
    {
        /// <inheritdoc/>
        public override Type BehaviourComponent => typeof(T);
    }
}
