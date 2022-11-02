using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace TentaPApi.RestControllers
{
    [Route("[controller]")]
    [ApiController]
    public class TamaPluggiController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendar()
        {
            Calendar calendar = new Calendar();

            CalendarEvent calendarEvent = new CalendarEvent
            {
                Summary = "Tamapluggi test event",
                Description = "This is a test event for tamapluggi",
                Start = new CalDateTime(2022, 3, 15, 12, 0, 0),
                End = new CalDateTime(2021, 3, 15, 15, 0, 0)
            };

            calendar.Events.Add(calendarEvent);

            CalendarSerializer iCalSerializer = new CalendarSerializer();
            string result = iCalSerializer.SerializeToString(calendar);

            return File(Encoding.ASCII.GetBytes(result), "text/calendar", "calendar.ics");
        }
    }
}
