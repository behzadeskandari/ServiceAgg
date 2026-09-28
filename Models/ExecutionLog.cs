namespace ServiceAggregator.Api.Models
{
    public class ExecutionLog
    {
        public Guid Id { get; set; }
        public Guid TrackerId { get; set; }
        public Guid AggretageId { get; set; }
        public Guid StepId { get; set; }
        public Guid? ServiceId { get; set; }
        public string? RequestJson { get; set; }
        public string? ResponseJson { get; set; }
        public int? ResponseStatusCode { get; set; }
        public DateTime ExecutionDate { get; set; }
        public long? Duration { get; set; }
    }
}
