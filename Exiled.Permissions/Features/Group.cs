﻿// -----------------------------------------------------------------------
// <copyright file="Group.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Features
{
    using System.Collections.Generic;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Represents a player's group.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Gets or sets a value indicating whether group is the default one or not.
        /// </summary>
        [YamlMember(Alias = "default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the group inheritance.
        /// </summary>
        public List<string> Inheritance { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the group permissions.
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
