using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiBroker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("{ip}")]
        public async Task<IActionResult> Get(string ip)
        {
            try
            {
                var location = await _locationService.GetLocationFromIPAsync(ip);
                return Ok(location);
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { error = ex.Message });
            }
        }
    }
}
