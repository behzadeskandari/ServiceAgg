using ServiceAggregator.Api.Models;
using System.Text.Json;
namespace ServiceAggregator.Api.Helpers;
public static class MapperHelper
{
    public static Dictionary<string, object?> Map(
        Aggregate aggregate,
        List<AggregateStepMapping> inputMappings,
        JsonElement aggregateInput,
        Dictionary<string, JsonElement> stepOutputs)
    {
        var stepInput = new Dictionary<string, object?>();

        foreach (var map in inputMappings)
        {
            object? value = null;

            if (map.Source == "AggregateInput")
            {
                if (aggregateInput.TryGetProperty(map.SourceField, out var val))
                    value = ExtractValue(val);
            }
            else if (map.Source == "StepOutput" && map.InputStepId.HasValue)
            {
                var prevOutput = stepOutputs[aggregate.Steps.Find(r => r.Id == map.InputStepId).StepName];
                value = ExtractValue(prevOutput, map.SourceField);
            }
            else if (map.Source == "Constant")
            {
                value = ExtractValue(map.Value, map.ValueType);
            }

            stepInput[map.TargetField] = value;
        }

        return stepInput;
    }

    /*public static Dictionary<string, object?> MapOutputs(
        Aggregate aggregate,
        List<AggregateStepMapping>? outputMappings,
        Dictionary<string, JsonElement> stepOutputs)
    {
        var result = new Dictionary<string, object?>();

        foreach (var map in outputMappings)
        {
            object? value = null;

            if (map.Source == "AggregateInput")
            {
                if (aggregateInput.TryGetProperty(map.SourceField, out var val))
                    value = ExtractValue(val);
            }
            else if (map.Source == "StepOutput" && map.InputStepId.HasValue)
            {
                var prevOutput = stepOutputs[aggregate.Steps.Find(r => r.Id == map.InputStepId).StepName];
                value = ExtractValue(prevOutput, map.SourceField);
            }
            else if (map.Source == "Constant")
            {
                value = ExtractValue(map.Value, map.ValueType);
            }

            stepInput[map.TargetField] = value;
        }

        return result;
    }*/
    public static object? ExtractValue(JsonElement val, string sourceField)
    {
        if (sourceField.Contains("."))
        {
            var fields = sourceField.Split('.');
            foreach (var field in fields)
            {
                var f = field;
                var index = -1;
                if (field.Contains("["))
                {
                    f = field.Substring(0, field.IndexOf('['));
                    index = Convert.ToInt32(field.Substring(field.IndexOf('[') + 1, field.IndexOf(']') - field.IndexOf('[') - 1));
                }
                if (index > -1)
                {
                    if (val.TryGetProperty(f, out val))
                    {
                        val = val[index];
                    }
                }
                else
                {
                    val.TryGetProperty(f, out val);
                }
            }
            return ExtractValue(val);
        }
        else if (val.TryGetProperty(sourceField, out var val2))
            return ExtractValue(val2);
        return null;
    }
    public static object? ExtractValue(JsonElement val)
    {
        return val.ValueKind switch
        {
            JsonValueKind.String => val.GetString(),
            JsonValueKind.Number => val.TryGetInt64(out var l) ? l : val.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Object => JsonToDictionary(val),
            JsonValueKind.Array => val.EnumerateArray().Select(ExtractValue).ToList(),
            _ => null
        };
    }
    private static Type? ResolveType(string typeName)
    {
        return typeName.ToLower() switch
        {
            "string" => typeof(string),
            "int" => typeof(int),
            "int?" => typeof(int?),
            "long" => typeof(long),
            "long?" => typeof(long?),
            "double" => typeof(double),
            "double?" => typeof(double?),
            "decimal" => typeof(decimal),
            "decimal?" => typeof(decimal?),
            "bool" => typeof(bool),
            "bool?" => typeof(bool?),
            "object" => typeof(object),

            _ => Type.GetType(typeName) // برای class یا enum های custom
        };
    }

    private static object? ExtractValue(string value, string targetTypeName)
    {
        var targetType = ResolveType(targetTypeName);

        if (targetType == null)
            throw new ArgumentException($"Unknown type: {targetTypeName}");

        // string
        if (targetType == typeof(string))
            return value;

        // int
        if (targetType == typeof(int) || targetType == typeof(int?))
            return int.TryParse(value, out var i) ? i : null;

        // long
        if (targetType == typeof(long) || targetType == typeof(long?))
            return long.TryParse(value, out var l) ? l : null;

        // double
        if (targetType == typeof(double) || targetType == typeof(double?))
            return double.TryParse(value, out var d) ? d : null;

        // decimal
        if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            return decimal.TryParse(value, out var m) ? m : null;

        // bool
        if (targetType == typeof(bool) || targetType == typeof(bool?))
            return bool.TryParse(value, out var b) ? b : null;

        // enum
        if (targetType.IsEnum)
            return Enum.TryParse(targetType, value, true, out var e) ? e : null;

        // JSON object / array
        if (value.Trim().StartsWith("{") || value.Trim().StartsWith("["))
            return JsonSerializer.Deserialize(value, targetType);

        // fallback
        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, object?> JsonToDictionary(object json)
    {
        JsonElement element;

        if (json is string jsonString)
        {
            using var doc = JsonDocument.Parse(jsonString);
            element = doc.RootElement.Clone();
        }
        else if (json is JsonElement jsonElement)
        {
            element = jsonElement;
        }
        else
        {
            throw new ArgumentException("Input must be either a JSON string or a JsonElement.");
        }

        return ConvertElementToDictionary(element);
    }
    private static Dictionary<string, object?> ConvertElementToDictionary(JsonElement element)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var prop in element.EnumerateObject())
        {
            dict[prop.Name] = ExtractValue(prop.Value);
        }

        return dict;
    }
}
