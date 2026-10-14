//using ServiceAggregator.Api.Models;
//using System.Text.Json;
//namespace ServiceAggregator.Api.Helpers;
//public static class MapperHelper
//{
//    public static Dictionary<string, object?> Map(
//    Aggregate aggregate,
//    List<AggregateStepMapping> inputMappings,
//    JsonElement aggregateInput,
//    Dictionary<string, JsonElement> stepOutputs)
//    {
//        // اگر Mapping تعریف نشده، کل input را pass-through کن
//        if (inputMappings == null || inputMappings.Count == 0)
//        {
//            if (aggregateInput.ValueKind != JsonValueKind.Object)
//                return new Dictionary<string, object?>();

//            return aggregateInput
//                .EnumerateObject()
//                .ToDictionary(
//                    property => property.Name,
//                    property => ExtractValue(property.Value));
//        }

//        var stepInput = new Dictionary<string, object?>();

//        foreach (var map in inputMappings)
//        {
//            object? value = null;

//            if (map.Source.Equals(
//                    "AggregateInput",
//                    StringComparison.OrdinalIgnoreCase))
//            {
//                value = ExtractValue(
//                    aggregateInput,
//                    map.SourceField);
//            }
//            else if (map.Source.Equals(
//                         "StepOutput",
//                         StringComparison.OrdinalIgnoreCase)
//                     && map.InputStepId.HasValue)
//            {
//                var inputStep = aggregate.Steps
//                    .FirstOrDefault(x => x.Id == map.InputStepId.Value);

//                if (inputStep == null)
//                {
//                    throw new InvalidOperationException(
//                        $"Input step '{map.InputStepId}' was not found.");
//                }

//                if (!stepOutputs.TryGetValue(
//                        inputStep.StepName,
//                        out var previousOutput))
//                {
//                    throw new InvalidOperationException(
//                        $"Output for step '{inputStep.StepName}' was not found.");
//                }

//                value = ExtractValue(
//                    previousOutput,
//                    map.SourceField);
//            }
//            else if (map.Source.Equals(
//                         "Constant",
//                         StringComparison.OrdinalIgnoreCase))
//            {
//                value = ExtractValue(
//                    map.Value,
//                    map.ValueType);
//            }

//            stepInput[map.TargetField] = value;
//        }

//        return stepInput;
//    }
//    //public static Dictionary<string, object?> Map(
//    //    Aggregate aggregate,
//    //    List<AggregateStepMapping> inputMappings,
//    //    JsonElement aggregateInput,
//    //    Dictionary<string, JsonElement> stepOutputs)
//    //{
//    //    var stepInput = new Dictionary<string, object?>();

//    //    foreach (var map in inputMappings)
//    //    {
//    //        object? value = null;

//    //        if (map.Source == "AggregateInput")
//    //        {
//    //            if (aggregateInput.TryGetProperty(map.SourceField, out var val))
//    //                value = ExtractValue(val);
//    //        }
//    //        else if (map.Source == "StepOutput" && map.InputStepId.HasValue)
//    //        {
//    //            var prevOutput = stepOutputs[aggregate.Steps.Find(r => r.Id == map.InputStepId).StepName];
//    //            value = ExtractValue(prevOutput, map.SourceField);
//    //        }
//    //        else if (map.Source == "Constant")
//    //        {
//    //            value = ExtractValue(map.Value, map.ValueType);
//    //        }

//    //        stepInput[map.TargetField] = value;
//    //    }

//    //    return stepInput;
//    //}

//    /*public static Dictionary<string, object?> MapOutputs(
//        Aggregate aggregate,
//        List<AggregateStepMapping>? outputMappings,
//        Dictionary<string, JsonElement> stepOutputs)
//    {
//        var result = new Dictionary<string, object?>();

//        foreach (var map in outputMappings)
//        {
//            object? value = null;

//            if (map.Source == "AggregateInput")
//            {
//                if (aggregateInput.TryGetProperty(map.SourceField, out var val))
//                    value = ExtractValue(val);
//            }
//            else if (map.Source == "StepOutput" && map.InputStepId.HasValue)
//            {
//                var prevOutput = stepOutputs[aggregate.Steps.Find(r => r.Id == map.InputStepId).StepName];
//                value = ExtractValue(prevOutput, map.SourceField);
//            }
//            else if (map.Source == "Constant")
//            {
//                value = ExtractValue(map.Value, map.ValueType);
//            }

//            stepInput[map.TargetField] = value;
//        }

//        return result;
//    }*/
//    public static object? ExtractValue(JsonElement val, string sourceField)
//    {
//        if (sourceField.Contains("."))
//        {
//            var fields = sourceField.Split('.');
//            foreach (var field in fields)
//            {
//                var f = field;
//                var index = -1;
//                if (field.Contains("["))
//                {
//                    f = field.Substring(0, field.IndexOf('['));
//                    index = Convert.ToInt32(field.Substring(field.IndexOf('[') + 1, field.IndexOf(']') - field.IndexOf('[') - 1));
//                }
//                if (index > -1)
//                {
//                    if (val.TryGetProperty(f, out val))
//                    {
//                        val = val[index];
//                    }
//                }
//                else
//                {
//                    val.TryGetProperty(f, out val);
//                }
//            }
//            return ExtractValue(val);
//        }
//        else if (val.TryGetProperty(sourceField, out var val2))
//            return ExtractValue(val2);
//        return null;
//    }
//    public static object? ExtractValue(JsonElement val)
//    {
//        return val.ValueKind switch
//        {
//            JsonValueKind.String => val.GetString(),
//            JsonValueKind.Number => val.TryGetInt64(out var l) ? l : val.GetDouble(),
//            JsonValueKind.True => true,
//            JsonValueKind.False => false,
//            JsonValueKind.Object => JsonToDictionary(val),
//            JsonValueKind.Array => val.EnumerateArray().Select(ExtractValue).ToList(),
//            _ => null
//        };
//    }
//    private static Type? ResolveType(string typeName)
//    {
//        return typeName.ToLower() switch
//        {
//            "string" => typeof(string),
//            "int" => typeof(int),
//            "int?" => typeof(int?),
//            "long" => typeof(long),
//            "long?" => typeof(long?),
//            "double" => typeof(double),
//            "double?" => typeof(double?),
//            "decimal" => typeof(decimal),
//            "decimal?" => typeof(decimal?),
//            "bool" => typeof(bool),
//            "bool?" => typeof(bool?),
//            "object" => typeof(object),

//            _ => Type.GetType(typeName) // برای class یا enum های custom
//        };
//    }

//    private static object? ExtractValue(string value, string targetTypeName)
//    {
//        var targetType = ResolveType(targetTypeName);

//        if (targetType == null)
//            throw new ArgumentException($"Unknown type: {targetTypeName}");

//        // string
//        if (targetType == typeof(string))
//            return value;

//        // int
//        if (targetType == typeof(int) || targetType == typeof(int?))
//            return int.TryParse(value, out var i) ? i : null;

//        // long
//        if (targetType == typeof(long) || targetType == typeof(long?))
//            return long.TryParse(value, out var l) ? l : null;

//        // double
//        if (targetType == typeof(double) || targetType == typeof(double?))
//            return double.TryParse(value, out var d) ? d : null;

//        // decimal
//        if (targetType == typeof(decimal) || targetType == typeof(decimal?))
//            return decimal.TryParse(value, out var m) ? m : null;

//        // bool
//        if (targetType == typeof(bool) || targetType == typeof(bool?))
//            return bool.TryParse(value, out var b) ? b : null;

//        // enum
//        if (targetType.IsEnum)
//            return Enum.TryParse(targetType, value, true, out var e) ? e : null;

//        // JSON object / array
//        if (value.Trim().StartsWith("{") || value.Trim().StartsWith("["))
//            return JsonSerializer.Deserialize(value, targetType);

//        // fallback
//        try
//        {
//            return Convert.ChangeType(value, targetType);
//        }
//        catch
//        {
//            return null;
//        }
//    }

//    private static Dictionary<string, object?> JsonToDictionary(object json)
//    {
//        JsonElement element;

//        if (json is string jsonString)
//        {
//            using var doc = JsonDocument.Parse(jsonString);
//            element = doc.RootElement.Clone();
//        }
//        else if (json is JsonElement jsonElement)
//        {
//            element = jsonElement;
//        }
//        else
//        {
//            throw new ArgumentException("Input must be either a JSON string or a JsonElement.");
//        }

//        return ConvertElementToDictionary(element);
//    }
//    private static Dictionary<string, object?> ConvertElementToDictionary(JsonElement element)
//    {
//        var dict = new Dictionary<string, object?>();

//        foreach (var prop in element.EnumerateObject())
//        {
//            dict[prop.Name] = ExtractValue(prop.Value);
//        }

//        return dict;
//    }
//}

using ServiceAggregator.Api.Models;
using System.Globalization;
using System.Text.Json;

namespace ServiceAggregator.Api.Helpers;

public static class MapperHelper
{
    public static Dictionary<string, object?> Map(
        Aggregate aggregate,
        List<AggregateStepMapping>? inputMappings,
        JsonElement aggregateInput,
        Dictionary<string, JsonElement> stepOutputs)
    {
        var result = new Dictionary<string, object?>(
            StringComparer.OrdinalIgnoreCase);

        // ---------------------------------------------------------
        // No mappings:
        // Pass the aggregate input directly to the service.
        // ---------------------------------------------------------
        if (inputMappings == null || inputMappings.Count == 0)
        {
            if (aggregateInput.ValueKind != JsonValueKind.Object)
                return result;

            foreach (var property in aggregateInput.EnumerateObject())
            {
                result[property.Name] = ExtractValue(property.Value);
            }

            return result;
        }

        foreach (var map in inputMappings)
        {
            if (map == null)
                continue;

            object? value = null;

            switch (map.Source?.Trim().ToLowerInvariant())
            {
                // -------------------------------------------------
                // AggregateInput
                // Example:
                // Source      = AggregateInput
                // SourceField = cif
                // TargetField = cif
                // -------------------------------------------------
                case "aggregateinput":
                    {
                        value = ExtractValue(
                            aggregateInput,
                            map.SourceField);

                        break;
                    }

                // -------------------------------------------------
                // StepOutput
                // Example:
                //
                // InputStepId = previous step
                // SourceField =
                // result.customerBriefDetailInfoBeans[0].cif
                //
                // TargetField = code
                // -------------------------------------------------
                case "stepoutput":
                    {
                        if (!map.InputStepId.HasValue)
                        {
                            throw new InvalidOperationException(
                                $"StepOutput mapping '{map.Id}' does not have InputStepId.");
                        }

                        var inputStep = aggregate.Steps?
                            .FirstOrDefault(x => x.Id == map.InputStepId.Value);

                        if (inputStep == null)
                        {
                            throw new InvalidOperationException(
                                $"Input step '{map.InputStepId}' was not found in aggregate.");
                        }

                        if (string.IsNullOrWhiteSpace(inputStep.StepName))
                        {
                            throw new InvalidOperationException(
                                $"Input step '{inputStep.Id}' has no StepName.");
                        }

                        if (!stepOutputs.TryGetValue(
                                inputStep.StepName,
                                out var previousOutput))
                        {
                            throw new InvalidOperationException(
                                $"Output for step '{inputStep.StepName}' was not found. " +
                                $"Available outputs: {string.Join(", ", stepOutputs.Keys)}");
                        }

                        value = ExtractValue(
                            previousOutput,
                            map.SourceField);

                        break;
                    }

                // -------------------------------------------------
                // Constant
                // Example:
                //
                // Value     = -1
                // ValueType = Int
                // Target    = rsCode
                // -------------------------------------------------
                case "constant":
                    {
                        value = ExtractConstant(
                            map.Value,
                            map.ValueType);

                        break;
                    }

                default:
                    {
                        throw new InvalidOperationException(
                            $"Unsupported mapping source '{map.Source}' " +
                            $"for mapping '{map.Id}'.");
                    }
            }

            //result[map.TargetField] = value;

            if (!string.IsNullOrWhiteSpace(map.TargetField))
            {
                SetNestedValue(result, map.TargetField, value);
            }
        }

        return result;
    }

    // =============================================================
    // JSON PATH
    // =============================================================

    public static object? ExtractValue(
        JsonElement element,
        string? sourceField)
    {
        if (string.IsNullOrWhiteSpace(sourceField))
            return ExtractValue(element);

        var path = sourceField.Trim();

        // Support optional "$." prefix
        if (path.StartsWith("$."))
            path = path[2..];
        else if (path == "$")
            return ExtractValue(element);

        var current = element;

        foreach (var token in TokenizePath(path))
        {
            if (token.IsArrayIndex)
            {
                if (current.ValueKind != JsonValueKind.Array)
                    return null;

                if (token.ArrayIndex < 0 ||
                    token.ArrayIndex >= current.GetArrayLength())
                {
                    return null;
                }

                current = current[token.ArrayIndex];
                continue;
            }

            if (current.ValueKind != JsonValueKind.Object)
                return null;

            if (!current.TryGetProperty(
                    token.Name!,
                    out var next))
            {
                return null;
            }

            current = next;
        }

        return ExtractValue(current);
    }

    // =============================================================
    // JSON VALUE -> CLR VALUE
    // =============================================================

    public static object? ExtractValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String =>
                value.GetString(),

            JsonValueKind.Number =>
                ExtractNumber(value),

            JsonValueKind.True =>
                true,

            JsonValueKind.False =>
                false,

            JsonValueKind.Object =>
                JsonToDictionary(value),

            JsonValueKind.Array =>
                value.EnumerateArray()
                    .Select(ExtractValue)
                    .ToList(),

            JsonValueKind.Null =>
                null,

            JsonValueKind.Undefined =>
                null,

            _ =>
                null
        };
    }

    private static object ExtractNumber(JsonElement value)
    {
        if (value.TryGetInt32(out var intValue))
            return intValue;

        if (value.TryGetInt64(out var longValue))
            return longValue;

        if (value.TryGetDecimal(out var decimalValue))
            return decimalValue;

        return value.GetDouble();
    }

    // =============================================================
    // CONSTANT
    // =============================================================

    private static object? ExtractConstant(
        string? value,
        string? typeName)
    {
        if (value == null)
            return null;

        if (string.IsNullOrWhiteSpace(typeName))
            return value;

        var type = typeName.Trim().ToLowerInvariant();

        switch (type)
        {
            case "string":
                return value;

            case "int":
            case "int?":
                return int.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var intValue)
                    ? intValue
                    : null;

            case "number":
            case "long":
            case "long?":
                return long.TryParse(
                    value,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out var longValue)
                    ? longValue
                    : null;

            case "double":
            case "double?":
                return double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var doubleValue)
                    ? doubleValue
                    : null;

            case "decimal":
            case "decimal?":
                return decimal.TryParse(
                    value,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var decimalValue)
                    ? decimalValue
                    : null;

            case "bool":
            case "bool?":
                return bool.TryParse(
                    value,
                    out var boolValue)
                    ? boolValue
                    : null;

            case "object":
                return ParseJsonIfPossible(value);

            default:
                {
                    var resolvedType = ResolveType(typeName);

                    if (resolvedType == null)
                    {
                        throw new ArgumentException(
                            $"Unknown mapping value type '{typeName}'.");
                    }

                    if (resolvedType.IsEnum)
                    {
                        return Enum.TryParse(
                            resolvedType,
                            value,
                            true,
                            out var enumValue)
                            ? enumValue
                            : null;
                    }

                    if (value.TrimStart().StartsWith("{") ||
                        value.TrimStart().StartsWith("["))
                    {
                        return JsonSerializer.Deserialize(
                            value,
                            resolvedType);
                    }

                    try
                    {
                        return Convert.ChangeType(
                            value,
                            resolvedType,
                            CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return null;
                    }
                }
        }
    }

    // =============================================================
    // PATH TOKENIZER
    // =============================================================

    private static IEnumerable<PathToken> TokenizePath(string path)
    {
        var parts = path.Split(
            '.',
            StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var current = part;

            while (current.Length > 0)
            {
                var bracketStart = current.IndexOf('[');

                if (bracketStart < 0)
                {
                    yield return new PathToken
                    {
                        Name = current,
                        IsArrayIndex = false
                    };

                    break;
                }

                if (bracketStart > 0)
                {
                    yield return new PathToken
                    {
                        Name = current[..bracketStart],
                        IsArrayIndex = false
                    };
                }

                var bracketEnd = current.IndexOf(
                    ']',
                    bracketStart);

                if (bracketEnd < 0)
                {
                    throw new FormatException(
                        $"Invalid JSON path '{path}'.");
                }

                var indexText = current.Substring(
                    bracketStart + 1,
                    bracketEnd - bracketStart - 1);

                if (!int.TryParse(
                        indexText,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out var index))
                {
                    throw new FormatException(
                        $"Invalid array index '{indexText}' in path '{path}'.");
                }

                yield return new PathToken
                {
                    ArrayIndex = index,
                    IsArrayIndex = true
                };

                current = current[(bracketEnd + 1)..];
            }
        }
    }

    // =============================================================
    // JSON OBJECT
    // =============================================================

    private static Dictionary<string, object?> JsonToDictionary(
        JsonElement element)
    {
        var result = new Dictionary<string, object?>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var property in element.EnumerateObject())
        {
            result[property.Name] = ExtractValue(property.Value);
        }

        return result;
    }

    private static object? ParseJsonIfPossible(string value)
    {
        var trimmed = value.Trim();

        if (!trimmed.StartsWith("{") &&
            !trimmed.StartsWith("["))
        {
            return value;
        }

        using var document = JsonDocument.Parse(trimmed);

        return ExtractValue(document.RootElement);
    }

    // =============================================================
    // TYPE RESOLUTION
    // =============================================================

    private static Type? ResolveType(string typeName)
    {
        return typeName.ToLowerInvariant() switch
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

            _ => Type.GetType(typeName)
        };
    }

    /// <summary>
    /// Sets a value into a nested dictionary structure using paths like:
    /// ClientBatchID
    /// PostCodes[0].ClientRowID
    /// result.user.name
    /// </summary>
    private static void SetNestedValue(
        Dictionary<string, object?> root,
        string path,
        object? value)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        var tokens = TokenizePath(path).ToList();
        object current = root;

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            var isLast = i == tokens.Count - 1;

            if (token.IsArrayIndex)
            {
                if (current is not List<object?> list)
                    return;

                while (list.Count <= token.ArrayIndex)
                    list.Add(null);

                if (isLast)
                {
                    list[token.ArrayIndex] = value;
                }
                else
                {
                    if (list[token.ArrayIndex] == null)
                    {
                        // next token decides whether we need a dict or another list
                        var next = tokens[i + 1];
                        list[token.ArrayIndex] = next.IsArrayIndex
                            ? new List<object?>()
                            : new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    }
                    current = list[token.ArrayIndex]!;
                }
            }
            else
            {
                if (current is not Dictionary<string, object?> dict)
                    return;

                var key = token.Name!;

                if (isLast)
                {
                    dict[key] = value;
                }
                else
                {
                    if (!dict.TryGetValue(key, out var nextObj) || nextObj == null)
                    {
                        var next = tokens[i + 1];
                        nextObj = next.IsArrayIndex
                            ? new List<object?>()
                            : new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                        dict[key] = nextObj;
                    }
                    current = nextObj!;
                }
            }
        }
    }
    private sealed class PathToken
    {
        public string? Name { get; init; }

        public bool IsArrayIndex { get; init; }

        public int ArrayIndex { get; init; }
    }
}
