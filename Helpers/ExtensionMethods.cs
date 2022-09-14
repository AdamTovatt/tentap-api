using System;
using System.Collections.Generic;
using System.Linq;

namespace TentaPApi.Helpers
{
    public static class ExtensionMethods
    {
        private static Random random;

        public static T TakeRandom<T>(this IEnumerable<T> elements)
        {
            if (random == null)
                random = new Random();

            return elements.ElementAt(random.Next(elements.Count()));
        }
    }
}
