using ServiceAggregator.Api.Models;

namespace ServiceAggregator.Api.Services
{
    public interface IServiceMappingService
    {
        Task<List<ServiceMappingDto>> GetByServiceIdAsync(Guid serviceId);
    }

    public class ServiceMappingDto
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public MappingDirection Direction { get; set; }      // 1 = Input, 2 = Output
        public Guid? ParentId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
