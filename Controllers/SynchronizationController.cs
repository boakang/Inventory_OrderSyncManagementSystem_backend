using Inventory_OrderSyncManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Inventory_OrderSyncManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SynchronizationController : ControllerBase
    {
        private readonly SynchronizationService _synchronizationService;

        public SynchronizationController(SynchronizationService synchronizationService)
        {
            _synchronizationService = synchronizationService;
        }

        [HttpGet("GetUpdatedData")]
        public IActionResult GetUpdatedData([FromQuery] string lastModified)
        {
            var updatedData = _synchronizationService.GetUpdatedData(lastModified);
            return Ok(updatedData);
        }

        [HttpPost("UploadChanges")]
        public IActionResult UploadChanges([FromBody] IEnumerable<object> changes)
        {
            _synchronizationService.UploadChanges(changes);
            return Ok("Changes uploaded successfully.");
        }
    }
}