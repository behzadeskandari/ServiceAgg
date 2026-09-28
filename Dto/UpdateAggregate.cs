namespace ServiceAggregator.Api.Dto;
public class UpdateAggregate
{
    /// <summary>
    /// شناسه ترکیب
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// شناسه اولین مرحله
    /// </summary>
    public Guid FirstStepId { get; set; }
    /// <summary>
    /// عنوان ترکیب
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// شرح ترکیب
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// وضعیت
    /// </summary>
    public bool Status { get; set; }
}
