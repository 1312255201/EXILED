// -----------------------------------------------------------------------
// <copyright file="Rank.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags
{
    /// <summary>
    /// Represents all existing ranks.
    /// </summary>
    public enum RankKind
    {
        /// <summary>
        /// Nothing.
        /// </summary>
        None,

        /// <summary>
        /// Exiled Developer.
        /// </summary>
        Dev = 1,

        /// <summary>
        /// Exiled Contributor.
        /// </summary>
        Contributor = 2,

        /// <summary>
        /// Exiled Plugin Developer.
        /// </summary>
        PluginDev = 3,
    }

    /// <summary>
    /// An object representing Exiled Credit ranks.
    /// </summary>
    public class Rank
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rank"/> class.
        /// </summary>
        /// <param name="name">The name of the rank.</param>
        /// <param name="color">The name of the rank's color.</param>
        /// <param name="hexValue">The hex color value of the rank's color.</param>
        public Rank(string name, string color, string hexValue)
        {
            Name = name;
            Color = color;
            HexValue = hexValue;
        }

        /// <summary>
        /// Gets a value representing the ranks name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value representing the ranks color.
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// Gets a value representing the rank's color as a hex value.
        /// </summary>
        public string HexValue { get; }
    }
}
