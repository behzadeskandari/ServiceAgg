using System.Buffers;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Infrastructure;
using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Helpers;
using ServiceAggregator.Api.Models;
using ServiceAggregator.Api.Services;

namespace ServiceAggregator.Api.Controllers;


[ApiController]
[Route("api/execute")]
public class ExecuteController : ControllerBase
{
    private readonly ServiceAggregatorContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ExecutionLogService _executionLogService;
    private readonly IConfiguration _configuration;

    public ExecuteController(IConfiguration configuration, ServiceAggregatorContext context, IHttpClientFactory httpClientFactory, ExecutionLogService executionLogService)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _executionLogService = executionLogService;
    }

    [HttpPost("ExecuteFlowAutomation/{aggregateName}")]
    public async Task<IActionResult> ExecuteFlowAutomation(string aggregateName, [FromBody] JsonElement input)
    {
        var aggByName = await _context.Aggregates
            .AsNoTracking()
            .Include(a => a.Steps)
            .ThenInclude(s => s.Service)
            .Include(a => a.Steps)
            .ThenInclude(s => s.Mappings)
            .FirstOrDefaultAsync(a => a.Name == aggregateName);
        if (aggByName == null)
            return NotFound("Aggregate Not Found");

        var result = await Execute(aggByName.Id, input);
        return result;
    }

    [HttpPost("{aggregateId}")]
    public async Task<IActionResult> Execute(Guid aggregateId, [FromBody] JsonElement input)
    {
        var apiKey = _configuration["ExternalApi:ApiKey"]
          ?? throw new InvalidOperationException("ExternalApi:ApiKey is not configured.");

        var trackerId = Guid.NewGuid();
        var aggregate = await _context.Aggregates
            .AsNoTracking()
            .Include(a => a.Steps)
            .ThenInclude(s => s.Service)
            .Include(a => a.Steps)
            .ThenInclude(s => s.Mappings)
            .FirstOrDefaultAsync(a => a.Id == aggregateId);
        if (aggregate == null)
            return NotFound("Aggregate not found!");
        var steps = aggregate.Steps.ToDictionary(s => s.Id);
        if (!aggregate.FirstStepId.HasValue || !steps.TryGetValue(aggregate.FirstStepId.Value, out var step))
            return NotFound("First Step not found!");
        var client = _httpClientFactory.CreateClient("AggregateClient");
        if (!client.DefaultRequestHeaders.Contains("apikey"))
            client.DefaultRequestHeaders.Add("apikey", apiKey);
        Dictionary<string, object?> mappedOutput = new();
        var stepOutputs = new Dictionary<string, JsonElement>();
        Dictionary<string, object?> finalOutput = new();
        while (step != null)
        {
            if (step.Service != null)
            {
                await ExecuteServiceStep(
                    client,
                    step,
                    aggregate,
                    input,
                    stepOutputs,
                    trackerId,
                    aggregateId
                );

                step = step.NextStepId != null && steps.TryGetValue(step.NextStepId.Value, out var next)
                    ? next
                    : null;

                continue;
            }

            if (!string.IsNullOrWhiteSpace(step.Condition))
            {
                await _executionLogService.AddLog(
                    trackerId,
                    aggregateId,
                    step.Id,
                    null,
                    null,
                    null,
                    null,
                    null
                    );
                step = EvaluateCondition(step, stepOutputs, steps);
                continue;
            }

            await _executionLogService.AddLog(
                trackerId,
                aggregateId,
                step.Id,
                null,
                null,
                null,
                null,
                null
                );
            finalOutput = MapperHelper.Map(aggregate, step.Mappings!.Where(r => r.MappingType == "Body").ToList(),
                input, stepOutputs);
            break;
        }

        return Ok(new { trackerId, result = finalOutput });
    }


    [HttpGet("export/pdf/{aggregateName}")]
    public async Task<IActionResult> ExportDocumentationPdf(string aggregateName)
    {
        // فعال‌سازی لایسنس QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.UseSystemFonts = true;
        var aggregate = await _context.Aggregates
            .AsNoTracking()
            .Include(a => a.Steps)
                .ThenInclude(s => s.Service)
            .Include(a => a.Steps)
                .ThenInclude(s => s.Mappings)
            .FirstOrDefaultAsync(a => a.Name == aggregateName);

        if (aggregate == null)
            return NotFound("اگریگیت مورد نظر یافت نشد.");

        try
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);

                    // جهت چیدمان صفحه برای زبان‌های RTL
                    page.ContentFromRightToLeft();

                    // استفاده از فونت‌های استانداردی که به‌صورت پیش‌فرض روی Windows موجود هستند
                    page.DefaultTextStyle(x => x.FontFamily("Segoe UI", "Tahoma", "Arial").FontSize(10));

                    // هدر سند
                    page.Header().Column(col =>
                    {
                        col.Item().Text("مستندات اگریگیت (Aggregate Documentation)")
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        col.Item().PaddingTop(5).Row(row =>
                        {
                            row.RelativeItem().Text($"نام اگریگیت: {aggregate.Name}").Bold();
                            row.RelativeItem().AlignLeft().Text($"شناسه: {aggregate.Id}")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken1);
                        });

                        col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // بدنه اصلی
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text("مراحل اجرا و مپینگ‌ها (Execution Steps & Mappings)")
                            .FontSize(14)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        if (aggregate.Steps == null || !aggregate.Steps.Any())
                        {
                            col.Item().PaddingTop(10).Text("هیچ گامی برای این اگریگیت تعریف نشده است.").Italic();
                        }
                        else
                        {
                            foreach (var step in aggregate.Steps)
                            {
                                col.Item().PaddingTop(15).Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(10).Column(stepCol =>
                                {
                                    stepCol.Item().Text($"گام: {step.StepName ?? "بدون نام"}")
                                        .Bold()
                                        .FontSize(11)
                                        .FontColor(Colors.Blue.Darken2);

                                    stepCol.Item().Text($"شناسه گام: {step.Id}")
                                        .FontSize(8)
                                        .FontColor(Colors.Grey.Darken1);

                                    stepCol.Item().Text($"نام سرویس: {step.Service?.Name ?? "ثبت نشده"}");
                                    stepCol.Item().Text($"آدرس سرویس: {step.Service?.Url ?? "ثبت نشده"}").DirectionFromLeftToRight();

                                    stepCol.Item().PaddingTop(8);

                                    if (step.Mappings != null && step.Mappings.Any())
                                    {
                                        stepCol.Item().Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn();
                                                columns.RelativeColumn();
                                                columns.RelativeColumn();
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("نوع مپینگ").Bold();
                                                header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("فیلد مبدا").Bold();
                                                header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("فیلد مقصد").Bold();
                                            });

                                            foreach (var map in step.Mappings)
                                            {
                                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(map.MappingType ?? "-");
                                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(map.SourceField ?? "-");
                                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(map.TargetField ?? "-");
                                            }
                                        });
                                    }
                                    else
                                    {
                                        stepCol.Item().Text("هیچ مپینگی برای این گام پیکربندی نشده است.")
                                            .Italic()
                                            .FontColor(Colors.Grey.Darken1);
                                    }
                                });
                            }
                        }
                    });

                    // فوتر
                    page.Footer().AlignLeft().Text(x =>
                    {
                        x.Span("صفحه ");
                        x.CurrentPageNumber();
                        x.Span(" از ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Documentation.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpGet("export/excel/{aggregateName}")]
    public async Task<IActionResult> ExportDocumentationExcel(string aggregateName)
    {
        var aggregate = await _context.Aggregates
         .AsNoTracking()
         .Include(a => a.Steps)
             .ThenInclude(s => s.Service)
         .Include(a => a.Steps)
             .ThenInclude(s => s.Mappings)
         .FirstOrDefaultAsync(a => a.Name == aggregateName);

        if (aggregate == null)
            return NotFound("Aggregate Not Found");

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Documentation");

        // ۱. تنظیم هدرهای جدول
        worksheet.Cell(1, 1).Value = "نام تجمیع";
        worksheet.Cell(1, 2).Value = "نام گام ";
        worksheet.Cell(1, 3).Value = "نام سرویس";
        worksheet.Cell(1, 4).Value = "سرویس URL";
        worksheet.Cell(1, 5).Value = "نوع مپ کردن";
        worksheet.Cell(1, 6).Value = "Source Field";
        worksheet.Cell(1, 7).Value = "Target Field";

        // استایل‌دهی به ردیف هدر
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        int currentRow = 2;

        // ۲. پر کردن داده‌ها
        foreach (var step in aggregate.Steps)
        {
            if (step.Mappings != null && step.Mappings.Any())
            {
                foreach (var map in step.Mappings)
                {
                    worksheet.Cell(currentRow, 1).Value = aggregate.Name;
                    worksheet.Cell(currentRow, 2).Value = step.StepName ?? "-";
                    worksheet.Cell(currentRow, 3).Value = step.Service?.Name ?? "N/A";
                    worksheet.Cell(currentRow, 4).Value = step.Service?.Url ?? "N/A";
                    worksheet.Cell(currentRow, 5).Value = map.MappingType ?? "-";
                    worksheet.Cell(currentRow, 6).Value = map.SourceField ?? "-";
                    worksheet.Cell(currentRow, 7).Value = map.TargetField ?? "-";
                    currentRow++;
                }
            }
            else
            {
                worksheet.Cell(currentRow, 1).Value = aggregate.Name;
                worksheet.Cell(currentRow, 2).Value = step.StepName ?? "-";
                worksheet.Cell(currentRow, 3).Value = step.Service?.Name ?? "N/A";
                worksheet.Cell(currentRow, 4).Value = step.Service?.Url ?? "N/A";
                worksheet.Cell(currentRow, 5).Value = "مپینگ وجود ندارد";
                worksheet.Cell(currentRow, 6).Value = "-";
                worksheet.Cell(currentRow, 7).Value = "-";
                currentRow++;
            }
        }

        // تنظیم خودکار عرض ستون‌ها بر اساس محتوا
        worksheet.Columns().AdjustToContents();

        // ۳. تبدیل خروجی به MemoryStream و ارسال به کلاینت
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        // MimeType استاندارد برای فایل‌های xlsx
        return File(
            content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Doc.xlsx"
        );
    }


    private async Task ExecuteServiceStep(
        HttpClient client,
        AggregateStep step,
        Aggregate aggregate,
        JsonElement input,
        Dictionary<string, JsonElement> stepOutputs,
        Guid trackerId,
        Guid aggregateId)
    {



        //        Console.WriteLine();
        //        Console.WriteLine("========================================");
        //        Console.WriteLine($"STEP: {step.StepName}");
        //        Console.WriteLine("========================================");

        //        Console.WriteLine("ORIGINAL AGGREGATE INPUT:");
        //        Console.WriteLine(input.GetRawText());

        //        Console.WriteLine();
        //        Console.WriteLine("STEP OUTPUTS:");

        //        foreach (var output in stepOutputs)
        //        {
        //            Console.WriteLine(
        //                $"Step: {output.Key}"
        //            );

        //            Console.WriteLine(
        //                output.Value.GetRawText()
        //            );
        //        }

        //        Console.WriteLine();
        //        Console.WriteLine("BODY MAPPINGS:");


        //        Console.WriteLine();
        //Console.WriteLine("MAPPED INPUT:");


        //Console.WriteLine("========================================");

        //var mappedInput = MapperHelper.Map(aggregate, step.Mappings!.Where(r => r.MappingType == "Body").ToList(), input, stepOutputs);
        var bodyMappings = step.Mappings?
        .Where(r => r.MappingType == "Body")
        .ToList()
        ?? new List<AggregateStepMapping>();

        var mappedInput = MapperHelper.Map(
            aggregate,
            bodyMappings,
            input,
            stepOutputs);


        var json = JsonSerializer.Serialize(mappedInput);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        //        Console.WriteLine(
        //    JsonSerializer.Serialize(
        //        mappedInput,
        //        new JsonSerializerOptions
        //        {
        //            WriteIndented = true
        //        }
        //    )
        //);
        //foreach (var mapping in bodyMappings)
        //{
        //    Console.WriteLine(
        //        $"Source       = {mapping.Source}"
        //    );

        //    Console.WriteLine(
        //        $"SourceField  = {mapping.SourceField}"
        //    );

        //    Console.WriteLine(
        //        $"TargetField  = {mapping.TargetField}"
        //    );

        //    Console.WriteLine(
        //        $"Value        = {mapping.Value}"
        //    );

        //    Console.WriteLine(
        //        $"InputStepId  = {mapping.InputStepId}"
        //    );

        //    Console.WriteLine("----------------------------------------");
        //}

        try
        {
            Stopwatch sw = new Stopwatch();
            var url = step.Service.Url;
            sw.Start();
            if (!string.IsNullOrWhiteSpace(step.Service.AuthenticationMode) &&
                step.Service!.AuthenticationMode.Equals("QueryString", StringComparison.OrdinalIgnoreCase))
            {
                var map = step.Mappings!.FirstOrDefault(r => r.MappingType == "Authentication");
                if (map != null)
                {
                    var prevOutput = stepOutputs[aggregate.Steps.Find(r => r.Id == map.InputStepId).StepName];
                    var value = MapperHelper.ExtractValue(prevOutput, map.SourceField);
                    if (value != null)
                        url += "?" + step.Service.AuthenticationFormat.Replace(map.TargetField, value.ToString(), StringComparison.OrdinalIgnoreCase);
                }
            }
            HttpResponseMessage response;
            // --- SOAP Execution using existing properties ---
            if (step.Service.Type.Equals("SOAP", StringComparison.OrdinalIgnoreCase))
            {
                // Reusing AuthenticationFormat as the SOAPAction configuration container if available,
                // or defaulting to a structural fallback name matching the step
                string soapAction = !string.IsNullOrWhiteSpace(step.Service.AuthenticationFormat)
                    ? step.Service.AuthenticationFormat
                    : $"{url}/{step.StepName}";

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                // Sending the actual mapped json structure inside the XML text payload wrapper
                request.Content = new StringContent(json, Encoding.UTF8, "text/xml");

                // Adding required SOAP action headers
                if (!request.Headers.Contains("SOAPAction"))
                {
                    request.Headers.Add("SOAPAction", soapAction.Contains("\"") ? soapAction : $"\"{soapAction}\"");
                }
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));

                response = await client.SendAsync(request);
            }
            // --- REST GET Execution ---
            else if (step.Service.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                response = await client.GetAsync(url);
            }
            // --- REST POST (or other methods) Execution ---
            else
            {
                response = await client.PostAsync(url, content);
            }

            // --- Response Parsing Step ---
            JsonElement respJson;
            if (step.Service.Type.Equals("SOAP", StringComparison.OrdinalIgnoreCase))
            {
                string rawResponse = await response.Content.ReadAsStringAsync();

                // Checking if the SOAP endpoint returned JSON wrapped inside text/xml
                // If it's valid JSON directly, parse it. Otherwise, turn the payload into string tokens.
                if ((rawResponse.Trim().StartsWith("{") && rawResponse.Trim().EndsWith("}")) ||
                    (rawResponse.Trim().StartsWith("[") && rawResponse.Trim().EndsWith("]")))
                {
                    respJson = JsonSerializer.Deserialize<JsonElement>(rawResponse);
                }
                else
                {
                    // Encapsulate XML / non-JSON string results securely as a Json value so downstream steps don't break
                    respJson = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(rawResponse));
                }
            }
            else
            {
                respJson = await response.Content.ReadFromJsonAsync<JsonElement>();
            }
            sw.Stop();

            stepOutputs[step.StepName] = respJson;

            await _executionLogService.AddLog(
                trackerId,
                aggregateId,
                step.Id,
                step.ServiceId!.Value,
                json,
                respJson.ToString(),
                (int)response.StatusCode,
                sw.ElapsedMilliseconds
            );
        }
        catch (Exception ex)
        {
            await _executionLogService.AddLog(
                trackerId,
                aggregateId,
                step.Id,
                step.ServiceId!.Value,
                json,
                ex.Message,
                500,
                0
            );

            throw;
        }
    }
    private AggregateStep? EvaluateCondition(
        AggregateStep step,
        Dictionary<string, JsonElement> stepOutputs,
        Dictionary<Guid, AggregateStep> steps)
    {
        var condition = step.Condition!;
        var allowedVariables = step.ConditionParameters.Split(',');

        var variables = new Dictionary<string, object>();

        foreach (var variable in allowedVariables)
        {
            var parts = variable.Split('.');
            var value = MapperHelper.ExtractValue(
                stepOutputs[parts[0]].GetProperty(parts[1])
            );

            variables[variable] = value;
            condition = condition.Replace(variable, value.ToString());
        }

        var result = DynamicConditionEvaluator.Evaluate(condition, variables, allowedVariables);

        var nextId = result ? step.TrueStepId : step.FalseStepId;
        return nextId != null && steps.TryGetValue(nextId.Value, out var next) ? next : null;
    }


    private string GenerateDocumentationHtml(Aggregate aggregate)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html><head><style>");
        sb.AppendLine("body { font-family: Tahoma, Arial, sans-serif; margin: 20px; direction: ltr; }");
        sb.AppendLine("table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
        sb.AppendLine("th, td { border: 1px solid #dddddd; text-align: left; padding: 8px; }");
        sb.AppendLine("th { background-color: #f2f2f2; }");
        sb.AppendLine("h1, h2 { color: #333; }");
        sb.AppendLine("</style></head><body>");

        sb.AppendLine($"<h1>Aggregate Documentation: {aggregate.Name}</h1>");
        sb.AppendLine($"<p><strong>Aggregate ID:</strong> {aggregate.Id}</p>");

        sb.AppendLine("<h2>Execution Steps & Mappings</h2>");

        foreach (var step in aggregate.Steps)
        {
            sb.AppendLine($"<h3>Step: {step.StepName} (Order / ID: {step.Id})</h3>");
            sb.AppendLine($"<p><strong>Service Name:</strong> {step.Service?.Name ?? "N/A"}</p>");
            sb.AppendLine($"<p><strong>Service URL:</strong> {step.Service?.Url ?? "N/A"}</p>");

            if (step.Mappings != null && step.Mappings.Any())
            {
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>Mapping Type</th><th>Source Field</th><th>Target Field</th></tr>");
                foreach (var map in step.Mappings)
                {
                    sb.AppendLine($"<tr><td>{map.MappingType}</td><td>{map.SourceField}</td><td>{map.TargetField}</td></tr>");
                }
                sb.AppendLine("</table>");
            }
            else
            {
                sb.AppendLine("<p><em>No Mappings configured for this step.</em></p>");
            }
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }
}