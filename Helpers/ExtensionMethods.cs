using Sakur.WebApiUtilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
    }
}
