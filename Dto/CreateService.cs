namespace ServiceAggregator.Api.Dto;
public class CreateService
{
    /// <summary>
    /// نام سرویس
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// url سرویس
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// متد سرویس
    /// </summary>
    public string Method { get; set; } = "POST"; // GET or POST
    /// <summary>
    /// نوع سرویس
    /// </summary>
    public string Type { get; set; } = "REST"; // SOAP
    /// <summary>
    /// نوع احراز هویت
    /// </summary>
    public string? AuthenticationMode { get; set; }//Header Or QueryString
    /// <summary>
    /// فرمت احراز هویت
    /// </summary>
    public string? AuthenticationFormat { get; set; }
}
