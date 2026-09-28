namespace ServiceAggregator.Api.Dto;
public class CreateAggregate
{
    /// <summary>
    /// عنوان ترکیب
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// شرح ترکیب
    /// </summary>
    public string Description { get; set; } = "";
}
