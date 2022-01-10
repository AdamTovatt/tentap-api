using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TentaPApi.Data;
using TentaPApi.Helpers;
using TentaPApi.Managers;

namespace TentaPApi.RestControllers
{
    [Route("resource/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string publicId)
        {
            GetResourceResult resource = await CloudinaryHelper.GetCloudinary().GetResourceAsync(publicId);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource.Url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (MemoryStream memory = new MemoryStream())
            {
                await stream.CopyToAsync(memory);
                return File(memory.ToArray(), "image/jpeg");
            }
        }
    }
}
