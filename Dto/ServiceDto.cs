using ServiceAggregator.Api.Models;
using System.Text.Json.Serialization;
namespace ServiceAggregator.Api.Dto;
public class ServiceDto
{
    /// <summary>
    /// شناسه سرویس
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// عنوان سرویس
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
    /// وضعیت
    /// </summary>
    public bool Status { get; set; }
    /// <summary>
    /// لیست مپینگ سرویس
    /// </summary>
    [JsonIgnore]
    public List<ServiceMapping> Mappings { get; set; } = new();
    /// <summary>
    /// لیست مپینگ ورودی ها
    /// </summary>
    public IReadOnlyList<ServiceMapping> Inputs { get; set; }
    /// <summary>
    /// لیست مپینگ خروجی ها
    /// </summary>
    public IReadOnlyList<ServiceMapping> Outputs { get; set; }
}
