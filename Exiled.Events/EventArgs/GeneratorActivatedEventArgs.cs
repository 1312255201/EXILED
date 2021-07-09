// -----------------------------------------------------------------------
// <copyright file="GeneratorActivatedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations after activating a generator.
    /// </summary>
    public class GeneratorActivatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorActivatedEventArgs"/> class.
        /// </summary>
        /// <param name="generator"><inheritdoc cref="Generator"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public GeneratorActivatedEventArgs(Generator079 generator, bool isAllowed = true)
        {
            Generator = generator;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the activated generator.
        /// </summary>
        public Generator079 Generator { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the generator can be activated or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
