using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakur.WebApiUtilities.Models;
using System.Text;
using System.Threading.Tasks;
using TentaPApi.Helpers;
using TentaPApi.Managers;
using TentaPApi.RequestBodies;

namespace TentaPApi.RestControllers
{
    [Route("[controller]")]
    [ApiController]
    public class TamaPluggiController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("{id}/calendar")]
        public async Task<IActionResult> GetCalendar(string id)
        {
            Calendar calendar = new Calendar();

            CalendarEvent calendarEvent = new CalendarEvent
            {
                Summary = "Tamapluggi test event",
                Description = "This is a test event for tamapluggi with id: " + id,
                Start = new CalDateTime(2022, 11, 5, 12, 0, 0),
                End = new CalDateTime(2022, 11, 5, 15, 0, 0)
            };

            calendar.Events.Add(calendarEvent);

            CalendarSerializer iCalSerializer = new CalendarSerializer();
            string result = iCalSerializer.SerializeToString(calendar);

            return File(Encoding.ASCII.GetBytes(result), "text/calendar", "calendar.ics");
        }

        [Authorize]
        [HttpPost("createNew")]
        public async Task<IActionResult> CreateNew([FromBody] CreateTamapluggiBody body)
        {
            try
            {
                if (!body.Valid)
                    return new ApiResponse(body.GetInvalidBodyMessage(), System.Net.HttpStatusCode.BadRequest);

                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                if (await database.GetTamapluggiForUserAsync() != null)
                    return new ApiResponse("Tamapluggi already exists for user!", System.Net.HttpStatusCode.BadRequest);

                return new ApiResponse(new { createdId = await database.CreateTamapluggiAsync(body.Name, body.StudyGoal, body.BreakDuration) }, System.Net.HttpStatusCode.OK);
            }
            catch(ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [Authorize]
        [HttpGet("fromUser")]
        public async Task<IActionResult> GetForUser()
        {
            try
            {
                DatabaseManager database = new DatabaseManager(UserHelper.GetClaims(User).GetUserId());

                return new ApiResponse(await database.GetTamapluggiForUserAsync(), System.Net.HttpStatusCode.OK);
            }
            catch(ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }
    }
}
