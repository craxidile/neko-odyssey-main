using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SpatiumInteractive.Libraries.Unity.Platform.Helpers;

namespace SpatiumInteractive.Libraries.Unity.Platform.Extensions
{
    /// <summary>
    /// Defines extensions for collection types
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Convert enumerable to readonly collection
        /// </summary>
        /// <typeparam name="T">Enumerable's type</typeparam>
        /// <param name="list">Enumerable</param>
        /// <returns>Readonly collection</returns>
        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                list = Enumerable.Empty<T>();
            }

            return new ReadOnlyCollection<T>(list.ToList());
        }

        /// <summary>
        /// Converts nullable list to an empty list
        /// </summary>
        /// <typeparam name="T">Enumerable's type</typeparam>
        /// <param name="list">collection</param>
        /// <returns>IEnumerable list</returns>
        public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Checkst whether list is null or empty
        /// </summary>
        /// <typeparam name="T">Enumerable's type</typeparam>
        /// <param name="list">collection</param>
        /// <returns>flag indicating whether list is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        /// <summary>
        /// Checks whether array is null or empty
        /// </summary>
        /// <typeparam name="T">array type</typeparam>
        /// <param name="array">array to test</param>
        /// <returns>flag indicating whether array is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || !array.Any();
        }

        public static void ForEach(this Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            ArrayTraverse walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }
    }
}
