using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Models;
using ServiceAggregator.Api.Services;

namespace ServiceAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/service-mapping")]
    public class ServiceMappingController : ControllerBase
    {
        private readonly ServiceAggregatorContext _context;
        private readonly IServiceMappingService _serviceMappingService;
        public ServiceMappingController(ServiceAggregatorContext context, IServiceMappingService serviceMappingService)
        {
            _context = context;
            _serviceMappingService = serviceMappingService;
        }

        [HttpGet("get/{serviceId:guid}")]
        public async Task<IActionResult> GetByServiceId(Guid serviceId)
        {
            var result = await _serviceMappingService.GetByServiceIdAsync(serviceId);

            if (result == null || !result.Any())
                return Ok(new List<ServiceMappingDto>()); // return empty list instead of 404

            return Ok(result);
        }


        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddServiceMapping([FromBody] AddServiceMapping addServiceMapping)
        {
            var newServiceMapping = new ServiceMapping()
            {
                Id = Guid.NewGuid(),
                ParentId = addServiceMapping.ParentId,
                ServiceId = addServiceMapping.ServiceId,
                Name = addServiceMapping.Name,
                Type = addServiceMapping.Type,
                Direction = addServiceMapping.Direction,
                Description = addServiceMapping.Description,
                CreateDate = DateTime.UtcNow
            };

            _context.ServiceMappings.Add(newServiceMapping);
            await _context.SaveChangesAsync();
            return Ok(newServiceMapping);
        }
        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateServiceMapping([FromBody] UpdateServiceMapping updateServiceMapping)
        {
            var serviceMapping = await _context.ServiceMappings.FirstOrDefaultAsync(r => r.Id == updateServiceMapping.Id);
            if (serviceMapping == null)
                return NotFound();
            serviceMapping.ParentId = updateServiceMapping.ParentId;
            serviceMapping.ServiceId = updateServiceMapping.ServiceId;
            serviceMapping.Name = updateServiceMapping.Name;
            serviceMapping.Type = updateServiceMapping.Type;
            serviceMapping.Description = updateServiceMapping.Description;
            serviceMapping.Direction = updateServiceMapping.Direction;
            serviceMapping.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(serviceMapping);
        }
        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteServiceMapping([FromBody] DeleteServiceMapping deleteServiceMapping)
        {
            var serviceMapping = await _context.ServiceMappings.FirstOrDefaultAsync(r => r.Id == deleteServiceMapping.Id);
            if (serviceMapping == null)
                return NotFound();
            _context.ServiceMappings.Remove(serviceMapping);
            await _context.SaveChangesAsync();
            return Ok(serviceMapping);
        }
    }
}
