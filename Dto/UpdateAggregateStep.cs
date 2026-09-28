namespace ServiceAggregator.Api.Dto;
public class UpdateAggregateStep
{
    /// <summary>
    /// شناسه مرحله
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// نام مرحله
    /// </summary>
    public string? StepName { get; set; }
    /// <summary>
    /// شناسه ترکیب
    /// </summary>
    public Guid AggregateId { get; set; }
    /// <summary>
    /// شناسه سرویس
    /// </summary>
    public Guid? ServiceId { get; set; }
    /// <summary>
    /// شناسه مرحله بعدی
    /// </summary>
    public Guid? NextStepId { get; set; }
    /// <summary>
    /// شناسه مرحله در صورت درست بودن شرط
    /// </summary>
    public Guid? TrueStepId { get; set; }
    /// <summary>
    /// شناسه مرحله در صورت نادرست بودن شرط
    /// </summary>
    public Guid? FalseStepId { get; set; }
    /// <summary>
    /// شرط
    /// </summary>
    public string? Condition { get; set; }
    /// <summary>
    /// پارامترهای استفاده شده در شرط
    /// </summary>
    public string? ConditionParameters { get; set; }
    /// <summary>
    /// وضعیت
    /// </summary>
    public bool Status { get; set; }
    /// <summary>
    /// محل افقی
    /// </summary>
    public decimal? PositionX { get; set; }
    /// <summary>
    /// محل عمودی
    /// </summary>
    public decimal? PositionY { get; set; }
}
