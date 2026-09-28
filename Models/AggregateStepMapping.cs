namespace ServiceAggregator.Api.Models;
public class AggregateStepMapping
{
    /// <summary>
    /// شناسه مپینگ
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// شناسه مرحله
    /// </summary>
    public Guid AggregateStepId { get; set; }
    /// <summary>
    /// نوع مپینگ
    /// </summary>
    public string? MappingType { get; set; } = "Body";//or Authentication
    /// <summary>
    /// شناسه مرحله ورودی
    /// </summary>
    public Guid? InputStepId { get; set; }
    /// <summary>
    /// منبع مورد استفاده
    /// </summary>
    public string Source { get; set; } = "AggregateInput";//or StepOutput or Constant
    /// <summary>
    /// نام فیلد مقصد
    /// </summary>
    public string TargetField { get; set; } = "";
    /// <summary>
    /// نام فیلد منبع
    /// </summary>
    public string SourceField { get; set; } = "";
    /// <summary>
    /// مقدار ثابت
    /// </summary>
    public string? Value { get; set; } = "";
    /// <summary>
    /// نوع فیلد
    /// </summary>
    public string ValueType { get; set; } = "";//String or Number Or True Or False Or Object Or Array
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
}
