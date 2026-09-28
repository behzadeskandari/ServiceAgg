using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Helpers;
using ServiceAggregator.Api.Models;
using SkiaSharp;
using System.Net;
using System.Threading;

namespace ServiceAggregator.Api.Controllers;
[ApiController]
[Route("api/aggregate")]
public class AggregateController : ControllerBase
{
    private readonly ServiceAggregatorContext _context;
    public AggregateController(ServiceAggregatorContext context) => _context = context;
    /// <summary>
    /// جزئیات بک ترکیب را برمیگرداند
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("get/{id:guid}")]
    public async Task<ActionResult<Aggregate>> GetAggregate(Guid id)
    {
        var aggregate = await _context.Aggregates
            .AsNoTracking()
            .FirstAsync(a => a.Id == id);

        aggregate.Mappings = await _context.AggregateMappings
            .Where(m => m.AggregateId == id)
            .ToListAsync();

        aggregate.Steps = await _context.AggregateSteps
            .Where(s => s.AggregateId == id)
            .Include(s => s.Service)
            .Include(s => s.Mappings)
            .ToListAsync();

        if (aggregate == null)
            return NotFound();

        return Ok(aggregate);
    }
    /// <summary>
    /// لیست ترکیب ها را با فیلتر ورودی و صفحه بندی برمیگرداند
    /// </summary>
    /// <param name="getAggregateWithPaginationQuery"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("get")]
    [ProducesResponseType(typeof(PaginatedList<GetAggregateWithPaginationDto>), (int)HttpStatusCode.OK)]
    public Task<PaginatedList<GetAggregateWithPaginationDto>> GetAggregates([FromQuery] GetAggregateWithPaginationQuery getAggregateWithPaginationQuery, CancellationToken cancellationToken)
    {
        var query = _context.Aggregates.AsQueryable();
        if (getAggregateWithPaginationQuery.Name != null)
        {
            query = query.Where(r => r.Name.Contains(getAggregateWithPaginationQuery.Name));
        }
        if (getAggregateWithPaginationQuery.Status != null)
        {
            query = query.Where(r => r.Status == getAggregateWithPaginationQuery.Status);
        }
        if (getAggregateWithPaginationQuery.CreateDate != null)
        {
            var createDate = getAggregateWithPaginationQuery.CreateDate.Value.Date;
            query = query.Where(r => r.CreateDate.Value.ToLocalTime() >= createDate && r.CreateDate.Value.ToLocalTime() < createDate.AddDays(1));
        }
        return query.OrderByDescending(r => r.CreateDate).
            Select(x => new GetAggregateWithPaginationDto()
            {
                Id = x.Id,
                Name = x.Name,
                Status = x.Status,
                CreateDate = x.CreateDate,
                UpdateDate = x.UpdateDate,
                Description = x.Description,
                FirstStepId = x.FirstStepId
            }).PaginatedListAsync(getAggregateWithPaginationQuery.PageIndex, getAggregateWithPaginationQuery.PageSize, cancellationToken);
    }
    /// <summary>
    /// یک ترکیب جدید می سازد
    /// </summary>
    /// <param name="createAggregate"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateAggregate([FromBody] CreateAggregate createAggregate)
    {
        var newAggregate = new Aggregate()
        {
            Id = Guid.NewGuid(),
            Name = createAggregate.Name,
            Description = createAggregate.Description,
            Status = true,
            CreateDate = DateTime.UtcNow,
        };
        _context.Aggregates.Add(newAggregate);
        await _context.SaveChangesAsync();
        return Ok(newAggregate);
    }
    /// <summary>
    /// یک ترکیب موجود را ویرایش می کند
    /// </summary>
    /// <param name="updateAggregate"></param>
    /// <returns></returns>
    [HttpPost("update")]
    public async Task<IActionResult> UpdateAggregate([FromBody] UpdateAggregate updateAggregate)
    {
        var aggregate = await _context.Aggregates.FirstOrDefaultAsync(r => r.Id == updateAggregate.Id);
        if (aggregate == null)
            return NotFound();
        aggregate.FirstStepId = updateAggregate.FirstStepId;
        aggregate.Name = updateAggregate.Name;
        aggregate.Description = updateAggregate.Description;
        aggregate.Status = updateAggregate.Status;
        aggregate.UpdateDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(updateAggregate);
    }
    /// <summary>
    /// یک ترکیب موجود را حذف می کند
    /// </summary>
    /// <param name="deleteAggregate"></param>
    /// <returns></returns>
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteAggregate([FromBody] DeleteAggregate deleteAggregate)
    {
        var aggregate = await _context.Aggregates.FirstOrDefaultAsync(r => r.Id == deleteAggregate.Id);
        if (aggregate == null)
            return NotFound();
        _context.Aggregates.Remove(aggregate);
        var steps = _context.AggregateSteps.Where(r => r.AggregateId == deleteAggregate.Id).ToList();
        _context.AggregateSteps.RemoveRange(steps);
        foreach (var step in steps)
        {
            var stepMappings = _context.AggregateStepMappings.Where(r => r.AggregateStepId == step.Id).ToList();
            _context.AggregateStepMappings.RemoveRange(stepMappings);
        }
        await _context.SaveChangesAsync();
        return Ok(deleteAggregate);
    }
}