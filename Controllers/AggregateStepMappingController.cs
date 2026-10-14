using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/aggregate-step-mapping")]
    public class AggregateStepMappingController : ControllerBase
    {
        private readonly ServiceAggregatorContext _context;
        public AggregateStepMappingController(ServiceAggregatorContext context) => _context = context;

        /// <summary>
        /// Get all mappings for a specific Aggregate Step
        /// </summary>
        [HttpGet("get-by-step/{aggregateStepId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByStepId(Guid aggregateStepId)
        {
            var mappings = await _context.AggregateStepMappings
                .AsNoTracking()
                .Where(x => x.AggregateStepId == aggregateStepId && x.Status)
                .OrderBy(x => x.CreateDate)
                .ToListAsync();

            return Ok(mappings);
        }

        /// <summary>
        /// Get all mappings for an entire Aggregate (all steps)
        /// </summary>
        [HttpGet("get-by-aggregate/{aggregateId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByAggregateId(Guid aggregateId)
        {
            var stepIds = await _context.AggregateSteps
                .AsNoTracking()
                .Where(s => s.AggregateId == aggregateId)
                .Select(s => s.Id)
                .ToListAsync();

            var mappings = await _context.AggregateStepMappings
                .AsNoTracking()
                .Where(x => stepIds.Contains(x.AggregateStepId) && x.Status)
                .OrderBy(x => x.CreateDate)
                .ToListAsync();

            return Ok(mappings);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAggregateStepMapping([FromBody] AddAggregateStepMapping addAggregateStepMapping)
        {
            var newAggregateStepMapping = new AggregateStepMapping()
            {
                Id = Guid.NewGuid(),
                AggregateStepId = addAggregateStepMapping.AggregateStepId,
                MappingType = addAggregateStepMapping.MappingType,
                InputStepId = addAggregateStepMapping.InputStepId,
                Source = addAggregateStepMapping.Source,
                TargetField = addAggregateStepMapping.TargetField,
                SourceField = addAggregateStepMapping.SourceField,
                Value = addAggregateStepMapping.Value,
                ValueType = addAggregateStepMapping.ValueType,
                Status = true,
                CreateDate = DateTime.UtcNow
            };
            _context.AggregateStepMappings.Add(newAggregateStepMapping);
            await _context.SaveChangesAsync();
            return Ok(newAggregateStepMapping);
        }
        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAggregateStepMapping([FromBody] UpdateAggregateStepMapping updateAggregateStepMapping)
        {
            var aggregateStepMapping = await _context.AggregateStepMappings.FirstOrDefaultAsync(r => r.Id == updateAggregateStepMapping.Id);
            if (aggregateStepMapping == null)
                return NotFound();
            aggregateStepMapping.AggregateStepId = updateAggregateStepMapping.AggregateStepId;
            aggregateStepMapping.MappingType = updateAggregateStepMapping.MappingType;
            aggregateStepMapping.InputStepId = updateAggregateStepMapping.InputStepId;
            aggregateStepMapping.Source = updateAggregateStepMapping.Source;
            aggregateStepMapping.TargetField = updateAggregateStepMapping.TargetField;
            aggregateStepMapping.SourceField = updateAggregateStepMapping.SourceField;
            aggregateStepMapping.Value = updateAggregateStepMapping.Value;
            aggregateStepMapping.ValueType = updateAggregateStepMapping.ValueType;
            aggregateStepMapping.Status = updateAggregateStepMapping.Status;
            aggregateStepMapping.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(updateAggregateStepMapping);
        }
        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAggregateStepMapping([FromBody] DeleteAggregateStepMapping deleteAggregateStepMapping)
        {
            var aggregateStepMapping = await _context.AggregateStepMappings.FirstOrDefaultAsync(r => r.Id == deleteAggregateStepMapping.Id);
            if (aggregateStepMapping == null)
                return NotFound();
            _context.AggregateStepMappings.Remove(aggregateStepMapping);
            await _context.SaveChangesAsync();
            return Ok(deleteAggregateStepMapping);
        }
    }
}
