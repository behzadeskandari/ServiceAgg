using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/step")]
    public class StepController : ControllerBase
    {
        private readonly ServiceAggregatorContext _context;
        public StepController(ServiceAggregatorContext context) => _context = context;
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAggregateStep([FromBody] AddAggregateStep addAggregateStep)
        {
            var newAggregateStep = new AggregateStep()
            {
                Id = Guid.NewGuid(),
                StepName = addAggregateStep.StepName,
                ServiceId = addAggregateStep.ServiceId,
                AggregateId = addAggregateStep.AggregateId,
                NextStepId = addAggregateStep.NextStepId,
                TrueStepId = addAggregateStep.TrueStepId,
                FalseStepId = addAggregateStep.FalseStepId,
                Condition = addAggregateStep.Condition,
                ConditionParameters = addAggregateStep.ConditionParameters,
                PositionX = addAggregateStep.PositionX,
                PositionY = addAggregateStep.PositionY,
                Status = true,
                CreateDate = DateTime.UtcNow
            };
            _context.AggregateSteps.Add(newAggregateStep);
            await _context.SaveChangesAsync();
            return Ok(newAggregateStep);
        }
        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAggregateStep([FromBody] UpdateAggregateStep updateAggregateStep)
        {
            var aggregateStep = await _context.AggregateSteps.FirstOrDefaultAsync(r => r.Id == updateAggregateStep.Id);
            if (aggregateStep == null)
                return NotFound();
            aggregateStep.StepName = updateAggregateStep.StepName;
            aggregateStep.ServiceId = updateAggregateStep.ServiceId;
            aggregateStep.AggregateId = updateAggregateStep.AggregateId;
            aggregateStep.NextStepId = updateAggregateStep.NextStepId;
            aggregateStep.TrueStepId = updateAggregateStep.TrueStepId;
            aggregateStep.FalseStepId = updateAggregateStep.FalseStepId;
            aggregateStep.Condition = updateAggregateStep.Condition;
            aggregateStep.ConditionParameters = updateAggregateStep.ConditionParameters;
            aggregateStep.Status = updateAggregateStep.Status;
            aggregateStep.PositionX = updateAggregateStep.PositionX;
            aggregateStep.PositionY = updateAggregateStep.PositionY;
            aggregateStep.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(aggregateStep);
        }
        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAggregateStep([FromBody] DeleteAggregateStep deleteAggregateStep)
        {
            var aggregateStep = await _context.AggregateSteps.FirstOrDefaultAsync(r => r.Id == deleteAggregateStep.Id);
            if (aggregateStep == null)
                return NotFound();
            _context.AggregateSteps.Remove(aggregateStep);
            var steps1 = _context.AggregateStepMappings.Where(r => r.AggregateStepId == deleteAggregateStep.Id).ToList();
            _context.AggregateStepMappings.RemoveRange(steps1);
            var steps2 = _context.AggregateSteps.Where(r => r.NextStepId == deleteAggregateStep.Id).ToList();
            foreach (var step in steps2)
            {
                step.NextStepId = null;
            }
            var steps3 = _context.AggregateSteps.Where(r => r.TrueStepId == deleteAggregateStep.Id).ToList();
            foreach (var step in steps3)
            {
                step.TrueStepId = null;
            }
            var steps4 = _context.AggregateSteps.Where(r => r.FalseStepId == deleteAggregateStep.Id).ToList();
            foreach (var step in steps4)
            {
                step.FalseStepId = null;
            }
            var steps5 = _context.Aggregates.Where(r => r.FirstStepId == deleteAggregateStep.Id).ToList();
            foreach (var aggregate in steps5)
            {
                aggregate.FirstStepId = null;
            }
            await _context.SaveChangesAsync();
            return Ok(aggregateStep);
        }
    }
}
