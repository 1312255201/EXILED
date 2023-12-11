// -----------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A set of extensions for easily interact with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Gets a random item from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to get the item from.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <returns>A random item from the <see cref="IEnumerable{T}"/>.</returns>
        public static T Random<T>(this IEnumerable<T> enumerable) =>
            enumerable is null || enumerable.Count() == 0 ? default : enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Count()));

        /// <summary>
        /// Gets a random item from an <see cref="IEnumerable{T}"/> given a condition.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to get the item from.</param>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/> elements.</typeparam>
        /// <param name="predicate">The specified condition.</param>
        /// <returns>A random item from the <see cref="IEnumerable{T}"/> matching the given condition.</returns>
        public static T Random<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) =>
            enumerable is null || enumerable.Count() == 0 ? default : enumerable.Where(predicate).Random();

        /// <summary>
        /// Retrieves a random item from an <see cref="IEnumerable{T}"/>.
        /// <para>
        /// <br>Unlike <see cref="Random{T}(IEnumerable{T})"/>, this method optimizes performance</br>
        /// <br>by pre-allocating memory for an array, resulting in faster computation and iteration.</br>
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to select the item from.</param>
        /// <returns>A randomly selected item from the <see cref="IEnumerable{T}"/>.</returns>
        public static T RandomAlloc<T>(this IEnumerable<T> enumerable)
        {
            T[] array = enumerable as T[] ?? enumerable.ToArray();
            return !array.Any() ? default : array.ElementAt(UnityEngine.Random.Range(0, array.Length));
        }

        /// <summary>
        /// Retrieves a random item from an <see cref="IEnumerable{T}"/> based on a specified condition.
        /// <para>
        /// <br>Unlike <see cref="Random{T}(IEnumerable{T}, Func{T, bool})"/>, this method optimizes performance</br>
        /// <br>by pre-allocating memory for an array, resulting in faster computation and iteration.</br>
        /// </para>
        /// </summary>
        /// <typeparam name="T">Type of elements in the <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to select the item from.</param>
        /// <param name="predicate">The specified condition for item selection.</param>
        /// <returns>A randomly selected item from the <see cref="IEnumerable{T}"/> that meets the given condition.</returns>
        public static T RandomAlloc<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            T[] arr = enumerable.Where(predicate) as T[] ?? enumerable.Where(predicate).ToArray();
            return !arr.Any() ? default : arr.ElementAt(UnityEngine.Random.Range(0, arr.Length));
        }

        /// <summary>
        /// Shuffles an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <see cref="IEnumerable{T}"/>.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="iterations">The amount of times to repeat the shuffle operation.</param>
        /// <returns>A shuffled version of the <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, int iterations = 1)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));
            if (iterations < 1)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            T[] array = enumerable.ToArray();
            array.Shuffle(iterations);
            return array;
        }
    }
}