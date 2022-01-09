using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TentaPApi.Data;
using TentaPApi.Managers;

namespace TentaPApi.RestControllers
{
    [Route("resource/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            ExerciseImage image = await new DatabaseManager().GetImage(id);
            return File(image.Data, "image/jpeg");
        }
    }
}
