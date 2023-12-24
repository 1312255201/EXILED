// -----------------------------------------------------------------------
// <copyright file="CommonExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// A set of extensions for common things.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Gets a random value from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable"><see cref="IEnumerable{T}"/> to be used to get a random value.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <returns>Returns a random value from <see cref="IEnumerable{T}"/>.</returns>
        [Obsolete("Use CollectionExtensions::Random<T>(IEnumerable<T>, Func<T, bool>) instead.", true)]
        public static T GetRandomValue<T>(this IEnumerable<T> enumerable) => CollectionExtensions.Random(enumerable);

        /// <summary>
        /// Gets a random value from an <see cref="IEnumerable{T}"/> that matches the provided condition.
        /// </summary>
        /// <param name="enumerable"><see cref="IEnumerable{T}"/> to be used to get a random value.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <param name="condition">The condition to require.</param>
        /// <returns>Returns a random value from <see cref="IEnumerable{T}"/>.</returns>
        [Obsolete("Use CollectionExtensions::Random<T>(IEnumerable<T>, Func<T, bool>) instead.", true)]
        public static T GetRandomValue<T>(this IEnumerable<T> enumerable, Func<T, bool> condition) => CollectionExtensions.Random(enumerable, condition);

        /// <summary>
        /// Modify the curve with the amount used.
        /// </summary>
        /// <param name="curve">The AnimationCurve to modify.</param>
        /// <param name="amount">The multiplier number.</param>
        /// <returns>The new modfied curve.</returns>
        public static AnimationCurve Multiply(this AnimationCurve curve, float amount)
        {
            for (var i = 0; i < curve.length; i++)
                curve.keys[i].value *= amount;

            return curve;
        }

        /// <summary>
        /// Modify the curve with the amount used.
        /// </summary>
        /// <param name="curve">The AnimationCurve to mofify.</param>
        /// <param name="amount">The add number.</param>
        /// <returns>The new modfied curve.</returns>
        public static AnimationCurve Add(this AnimationCurve curve, float amount)
        {
            for (var i = 0; i < curve.length; i++)
                curve.keys[i].value += amount;

            return curve;
        }
    }
}