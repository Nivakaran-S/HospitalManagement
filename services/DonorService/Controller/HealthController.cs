using Microsoft.AspNetCore.Mvc;


namespace DonorService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "DonorService",
            timestamp = DateTime.UtcNow
        });
    }
}