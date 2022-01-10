using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TentaPApi.Helpers
{
    public class CloudinaryHelper
    {
        public static Cloudinary GetCloudinary()
        {
            string cloud = EnvironmentHelper.GetEnvironmentVariable("CLOUDINARY_CLOUD");
            string apiKey = EnvironmentHelper.GetEnvironmentVariable("CLOUDINARY_KEY");
            string secret = EnvironmentHelper.GetEnvironmentVariable("CLOUDINARY_SECRET");
            return new Cloudinary(new Account(cloud, apiKey, secret));
        }
    }
}
