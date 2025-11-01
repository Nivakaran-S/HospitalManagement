using Microsoft.AspNetCore.Mvc;


namespace InventoryService.Controllers  
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            status = "Healthy",
            service = "InventoryService",
            timestamp = DateTime.UtcNow
        });
    }
}