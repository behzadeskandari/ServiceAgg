using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/aggregate-mapping")]
    public class AggregateMappingController : ControllerBase
    {
        private readonly ServiceAggregatorContext _context;
        public AggregateMappingController(ServiceAggregatorContext context) => _context = context;
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAggregateMapping([FromBody] AddAggregateMapping addAggregateMapping)
        {
            var newAggregateMapping = new AggregateMapping()
            {
                Id = Guid.NewGuid(),
                ParentId = addAggregateMapping.ParentId,
                AggregateId = addAggregateMapping.AggregateId,
                Name = addAggregateMapping.Name,
                Type = addAggregateMapping.Type,
                Description = addAggregateMapping.Description,
                CreateDate = DateTime.UtcNow
            };
            _context.AggregateMappings.Add(newAggregateMapping);
            await _context.SaveChangesAsync();
            return Ok(addAggregateMapping);
        }
        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAggregateMapping([FromBody] UpdateAggregateMapping updateAggregateMapping)
        {
            var aggregateMapping = await _context.AggregateMappings.FirstOrDefaultAsync(r => r.Id == updateAggregateMapping.Id);
            if (aggregateMapping == null)
                return NotFound();
            aggregateMapping.ParentId = updateAggregateMapping.ParentId;
            aggregateMapping.AggregateId = updateAggregateMapping.AggregateId;
            aggregateMapping.Name = updateAggregateMapping.Name;
            aggregateMapping.Type = updateAggregateMapping.Type;
            aggregateMapping.Description = updateAggregateMapping.Description;
            aggregateMapping.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(aggregateMapping);
        }
        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAggregateMapping([FromBody] DeleteAggregateMapping deleteAggregateMapping)
        {
            var aggregateMapping = await _context.AggregateMappings.FirstOrDefaultAsync(r => r.Id == deleteAggregateMapping.Id);
            if (aggregateMapping == null)
                return NotFound();
            _context.AggregateMappings.Remove(aggregateMapping);
            await _context.SaveChangesAsync();
            return Ok(aggregateMapping);
        }
    }
}
