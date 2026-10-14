using Microsoft.EntityFrameworkCore;
using ServiceAggregator.Api.Data;

namespace ServiceAggregator.Api.Services
{
    public class ServiceMappingService : IServiceMappingService
    {
        private readonly ServiceAggregatorContext _context;

        public ServiceMappingService(ServiceAggregatorContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceMappingDto>> GetByServiceIdAsync(Guid serviceId)
        {
            return await _context.Services
                         .AsNoTracking()
                         .Where(s => s.Id == serviceId)
                         .SelectMany(s => s.Mappings)
                         .OrderBy(m => m.Direction)
                         .ThenBy(m => m.Name)
                         .Select(m => new ServiceMappingDto
                         {
                             Id = m.Id,
                             ServiceId = m.ServiceId,
                             Name = m.Name,
                             Type = m.Type,
                             Description = m.Description,
                             Direction = m.Direction,
                             ParentId = m.ParentId,
                             CreateDate = m.CreateDate,
                             UpdateDate = m.UpdateDate
                         })
                         .ToListAsync();
        }
    }
}
