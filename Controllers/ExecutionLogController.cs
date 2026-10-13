using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Controllers
{
    [ApiController]
    [Route("api/executelog")]
    public class ExecutionLogController : ControllerBase
    {
        private readonly ServiceAggregatorContext _context;
        public ExecutionLogController(ServiceAggregatorContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
        }
        [HttpGet("get/{trackerid:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ExecutionLog>>> GetLogs(Guid trackerid)
        {
            return await _context.ExecutionLogs
                .AsNoTracking()
                .Where(a => a.TrackerId == trackerid)
                .OrderBy(r=>r.ExecutionDate)
                .ToListAsync();
        }
    }
}
