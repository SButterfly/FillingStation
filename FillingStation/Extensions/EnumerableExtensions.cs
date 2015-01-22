using System;
using System.Collections.Generic;
using System.Linq;
using FillingStation.Helpers;

namespace FillingStation.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random _random = Randomizer.GetInstance().Random;

        public static TSource Random<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.ElementAt(_random.Next(source.Count()));
        }
    }
}