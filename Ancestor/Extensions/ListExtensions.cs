using System;
using System.Collections.Generic;
using System.Linq;

namespace Ancestor.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> GetChunks<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();

            return Enumerable
                .Range(0, (int)Math.Ceiling(new decimal(array.Length) / chunkSize))
                .Select(i => array.Skip(i * chunkSize).Take(chunkSize).ToArray());
        }
        
        public static IEnumerable<string> GetChunks(this string text, int chunkSize)
        {
            return Enumerable.Range(0, (int)Math.Ceiling(new decimal(text.Length) / chunkSize))
                .Select(i => new string(text
                    .Skip(i * chunkSize)
                    .Take(chunkSize)
                    .ToArray()));
        }
    }
}
