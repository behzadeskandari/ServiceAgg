using System.Buffers;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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

    public ExecuteController(ServiceAggregatorContext context, IHttpClientFactory httpClientFactory, ExecutionLogService executionLogService)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _executionLogService = executionLogService;
    }

    [HttpPost("{aggregateId}")]
    public async Task<IActionResult> Execute(Guid aggregateId, [FromBody] JsonElement input)
    {
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
            client.DefaultRequestHeaders.Add("apikey", "LrU7n4ENc6zTHw1WZblBPgCJmhVD2XQxyYvR");
        Dictionary<string, object?> mappedOutput = new();
        var stepOutputs = new Dictionary<string, JsonElement>();
        Dictionary<string, object?> finalOutput = new();
        while (step != null )
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
    private async Task ExecuteServiceStep(
        HttpClient client,
        AggregateStep step,
        Aggregate aggregate,
        JsonElement input,
        Dictionary<string, JsonElement> stepOutputs,
        Guid trackerId,
        Guid aggregateId)
    {
        var mappedInput = MapperHelper.Map(aggregate, step.Mappings!.Where(r => r.MappingType == "Body").ToList(), input, stepOutputs);
        var json = JsonSerializer.Serialize(mappedInput);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

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
                        url += "?" + step.Service.AuthenticationFormat.Replace(map.TargetField, value.ToString(),StringComparison.OrdinalIgnoreCase);
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
}