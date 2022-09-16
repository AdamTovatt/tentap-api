using Sakur.WebApiUtilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TentaPApi.Models;

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

        public static int GetUserId(this Dictionary<string, string> claims)
        {
            if (!claims.ContainsKey("sub"))
                return 0;

            if (int.TryParse(claims["sub"], out int userId))
                return userId;

            throw new ApiException("invalid id in token object", HttpStatusCode.BadRequest);
        }

        public static int[] ToIntArray(this Difficulty[] original)
        {
            int[] result = new int[original.Length];

            for (int i = 0; i < original.Length; i++)
                result[i] = (int)original[i];

            return result;
        }
    }
}
