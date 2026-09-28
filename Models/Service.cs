using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceAggregator.Api.Models;
public class Service
{
    /// <summary>
    /// شناسه سرویس
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// نام سرویس
    /// </summary>
    public string Name { get; set; } = "";
    /// <summary>
    /// url سرویس
    /// </summary>
    public string Url { get; set; } = "";
    /// <summary>
    /// متد سرویس
    /// </summary>
    public string Method { get; set; } = "POST"; // GET or POST
    /// <summary>
    /// نوع سرویس
    /// </summary>
    public string Type { get; set; } = "REST";
    /// <summary>
    /// وضعیت
    /// </summary>
    public bool Status { get; set; } = true;
    /// <summary>
    /// لیست مپینگ
    /// </summary>
    public List<ServiceMapping> Mappings { get; set; } = new();
    /// <summary>
    /// تاریخ ایجاد
    /// </summary>
    public DateTime? CreateDate { get; set; }
    /// <summary>
    /// تاریخ تغییر
    /// </summary>
    public DateTime? UpdateDate { get; set; }
    /// <summary>
    /// نوع احراز هویت
    /// </summary>
    public string? AuthenticationMode { get; set; }//Header or QueryString
    /// <summary>
    /// فرمت احراز هویت
    /// </summary>
    public string? AuthenticationFormat { get; set; }
    /// <summary>
    /// لیست مپینگ ورودی ها
    /// </summary>
    [NotMapped]
    public IReadOnlyList<ServiceMapping> Inputs =>
        Mappings.Where(x => x.Direction == MappingDirection.Input).ToList();
    /// <summary>
    /// لیست مپینگ خروجی ها
    /// </summary>
    [NotMapped]
    public IReadOnlyList<ServiceMapping> Outputs =>
        Mappings.Where(x => x.Direction == MappingDirection.Output).ToList();
}