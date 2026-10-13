namespace ServiceAggregator.Api.Models;
public class Aggregate
{
    /// <summary>
    /// شناسه ترکیب
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// شناسه اولین مرحله
    /// </summary>
    public Guid? FirstStepId { get; set; }
    /// <summary>
    /// عنوان ترکیب
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// شرح ترکیب
    /// </summary>
    public string Description { get; set; } = "";
    /// <summary>
    /// وضعیت
    /// </summary>
    public bool Status { get; set; } = true;

    /// <summary>
    /// تاریخ ایجاد
    /// </summary>
    public DateTime? CreateDate { get; set; }
    /// <summary>
    /// تاریخ تغییر
    /// </summary>
    public DateTime? UpdateDate { get; set; }

    //public ServiceOperation? ServiceOperation { get; set; }

    //public AggregateStep? FirstStep { get; set; }
    /// <summary>
    /// لیست مراحل ترکیب
    /// </summary>
    public List<AggregateStep> Steps { get; set; } = new List<AggregateStep>();

    /// <summary>
    /// لیست مپینگ
    /// </summary>
    public List<AggregateMapping>? Mappings { get; set; } = new();
}
