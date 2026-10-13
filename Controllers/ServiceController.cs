using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Dto;
using ServiceAggregator.Api.Helpers;
using ServiceAggregator.Api.Migrations;
using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/service")]
    public class ServiceController(ServiceAggregatorContext context) : ControllerBase
    {
        private readonly ServiceAggregatorContext _context = context;

        [HttpGet("get")]
        [ProducesResponseType(typeof(PaginatedList<GetServiceWithPaginationDto>), (int)HttpStatusCode.OK)]
        [Authorize(Roles = "Admin")]
        public Task<PaginatedList<GetServiceWithPaginationDto>> GetServices([FromQuery] GetServiceWithPaginationQuery getServiceWithPaginationQuery, CancellationToken cancellationToken)
        {
            var query = _context.Services.AsQueryable();

            if (getServiceWithPaginationQuery.Name != null)
            {
                query = query.Where(r => r.Name.Contains(getServiceWithPaginationQuery.Name));
            }
            if (getServiceWithPaginationQuery.Url != null)
            {
                query = query.Where(r => r.Url.Contains(getServiceWithPaginationQuery.Url));
            }
            if (getServiceWithPaginationQuery.Status != null)
            {
                query = query.Where(r => r.Status == getServiceWithPaginationQuery.Status);
            }
            //var serviceIds = getServiceWithPaginationQuery.Inputs
            //.Select(input => input.ServiceId)
            //.ToList();

            //List<ServiceMapping> mappingService = new List<ServiceMapping>();
            //var hasMappings = _context.ServiceMappings
            //    .Any(mapping => serviceIds.Contains(mapping.ServiceId));

            //if (hasMappings)
            //{
            //    mappingService = _context.ServiceMappings.Where(mapping => serviceIds.Contains(mapping.ServiceId))
            //        .Select(mapping => mapping)
            //        .Distinct()
            //        .ToList();


            //}
            if (getServiceWithPaginationQuery.CreateDate != null)
            {
                var createDate = getServiceWithPaginationQuery.CreateDate.Value.Date;
                query = query.Where(r => r.CreateDate.Value.ToLocalTime() >= createDate && r.CreateDate.Value.ToLocalTime() < createDate.AddDays(1));
            }
            return query.OrderByDescending(r => r.CreateDate).
                Select(s => new GetServiceWithPaginationDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Url = s.Url,
                    Method = s.Method,
                    //Mappings = mappingService,
                    Type = s.Type,
                    Status = s.Status,
                    CreateDate = s.CreateDate,
                    UpdateDate = s.UpdateDate,
                    AuthenticationMode = s.AuthenticationMode,
                    AuthenticationFormat = s.AuthenticationFormat,
                }).PaginatedListAsync(getServiceWithPaginationQuery.PageIndex, getServiceWithPaginationQuery.PageSize, cancellationToken);
        }
        [HttpGet("get/{id:guid}")]
        public async Task<ActionResult<ServiceDto>> GetService(Guid id)
        {
            var result = await _context.Services
                .Where(s => s.Id == id)
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Url = s.Url,
                    Method = s.Method,
                    Type = s.Type,
                    Status = s.Status,
                    Inputs = s.Inputs,
                    Mappings = s.Mappings,
                    Outputs = s.Outputs
                })
                .FirstOrDefaultAsync();

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateService([FromBody] CreateService createService)
        {
            var newService = new Service()
            {
                Id = Guid.NewGuid(),
                Name = createService.Name,
                Url = createService.Url,
                Method = createService.Method,
                Type = createService.Type,
                Status = true,
                CreateDate = DateTime.UtcNow,
                AuthenticationMode = createService.AuthenticationMode,
                AuthenticationFormat = createService.AuthenticationFormat,
            };
            _context.Services.Add(newService);
            await _context.SaveChangesAsync();
            return Ok(newService);
        }
        [HttpPost("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateService([FromBody] UpdateService updateService)
        {
            var service = await _context.Services.FirstOrDefaultAsync(r => r.Id == updateService.Id);
            if (service == null)
                return NotFound();
            service.Name = updateService.Name;
            service.Url = updateService.Url;
            service.Method = updateService.Method;
            service.Type = updateService.Type;
            service.Status = updateService.Status;
            service.UpdateDate = DateTime.UtcNow;
            service.AuthenticationMode = updateService.AuthenticationMode;
            service.AuthenticationFormat = updateService.AuthenticationFormat;
            await _context.SaveChangesAsync();
            return Ok(service);
        }
        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteService([FromBody] DeleteService deleteService)
        {
            var service = await _context.Services.FirstOrDefaultAsync(r => r.Id == deleteService.Id);
            if (service == null)
                return NotFound();
            _context.Services.Remove(service);
            var serviceMappings = _context.ServiceMappings.Where(r => r.ServiceId == deleteService.Id).ToList();
            _context.ServiceMappings.RemoveRange(serviceMappings);
            var steps = _context.AggregateSteps.Where(r => r.ServiceId == deleteService.Id).ToList();
            _context.AggregateSteps.RemoveRange(steps);
            foreach (var step in steps)
            {
                var steps1 = _context.AggregateStepMappings.Where(r => r.AggregateStepId == step.Id).ToList();
                _context.AggregateStepMappings.RemoveRange(steps1);
                var steps2 = _context.AggregateSteps.Where(r => r.NextStepId == step.Id).ToList();
                foreach (var stepp in steps2)
                {
                    stepp.NextStepId = null;
                }
                var steps3 = _context.AggregateSteps.Where(r => r.TrueStepId == step.Id).ToList();
                foreach (var stepp in steps3)
                {
                    stepp.TrueStepId = null;
                }
                var steps4 = _context.AggregateSteps.Where(r => r.FalseStepId == step.Id).ToList();
                foreach (var stepp in steps4)
                {
                    stepp.FalseStepId = null;
                }
                var steps5 = _context.Aggregates.Where(r => r.FirstStepId == step.Id).ToList();
                foreach (var aggregate in steps5)
                {
                    aggregate.FirstStepId = null;
                }
            }
            await _context.SaveChangesAsync();
            return Ok(service);
        }
    }
}
