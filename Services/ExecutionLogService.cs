using ServiceAggregator.Api.Data;

namespace ServiceAggregator.Api.Services
{
    public class ExecutionLogService
    {
        private readonly ServiceAggregatorContext _context;
        public ExecutionLogService(ServiceAggregatorContext context) => _context = context;
        public async Task AddLog(Guid trackerId, Guid aggretageId, Guid stepId, Guid? serviceId, string? requestJson, string? responseJson, int? responseStatusCode, long? duration)
        {
            _context.ExecutionLogs.Add(new Models.ExecutionLog()
            {
                Id = Guid.NewGuid(),
                TrackerId = trackerId,
                AggretageId = aggretageId,
                StepId = stepId,
                ServiceId = serviceId,
                RequestJson = requestJson,
                ResponseJson = responseJson,
                ResponseStatusCode = responseStatusCode,
                ExecutionDate = DateTime.UtcNow,
                Duration = duration,
            });
            await _context.SaveChangesAsync();
        }
    }
}
