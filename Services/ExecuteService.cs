using ServiceAggregator.Api.Data;
using ServiceAggregator.Api.Helpers;
using System.Text;
using System.Text.Json;

namespace ServiceAggregator.Api.Services;

public class ExecuteService
{
    private readonly ServiceAggregatorContext _db;
    private readonly HttpClient _http;

    public ExecuteService(ServiceAggregatorContext db, IHttpClientFactory httpFactory)
    {
        _db = db;
        _http = httpFactory.CreateClient();
    }

    public async Task<object?> ExecuteAsync(Guid aggregateId, JsonElement input)
    {
        var aggregate = _db.Aggregates
            .Where(s => s.Id == aggregateId)
            .Select(s => new { s.Id, s.Name, s.Steps})
            .FirstOrDefault();

        if (aggregate == null) throw new Exception("Service not found");

        var context = new Dictionary<string, JsonElement>
        {
            { "input", input }
        };

        foreach (var step in aggregate.Steps.OrderBy(s => s.StepOrder))
        {
            var service = _db.Services.First(c => c.Id == step.ServiceId);

            var payload = MapperHelper.MapInputs(step.Mappings, input, context);

            string result;
            if (service.Type == "REST")
            {
                var req = new HttpRequestMessage(
                    new HttpMethod(service.Method),
                    service.Url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
                };
                var res = await _http.SendAsync(req);
                result = await res.Content.ReadAsStringAsync();
            }
            else
            {
                // TODO: SOAP call handling
                throw new NotImplementedException("SOAP not implemented yet");
            }

            var json = JsonSerializer.Deserialize<JsonElement>(result);
            MapperHelper.MapOutputs(step.Mappings, json);
        }

        return context.ContainsKey("output") ? context["output"] : context;
    }
}
