using Newtonsoft.Json.Linq;
namespace ServiceAggregator.Api.Helpers;
public static class StepMapper
{
    public static Dictionary<string, object> MapInputs(string inputMapping, Dictionary<string, object> contextData, string inputType)
    {
        var stepInput = new Dictionary<string, object>();
        if (string.IsNullOrEmpty(inputMapping)) return stepInput;

        var mapping = JObject.Parse(inputMapping);
        foreach (var prop in mapping.Properties())
        {
            var sourceKey = prop.Value.ToString();
            if (!contextData.ContainsKey(sourceKey)) continue;

            object value = contextData[sourceKey];

            switch (inputType.ToLower())
            {
                case "int":
                    value = Convert.ToInt32(value);
                    break;
                case "bool":
                    value = Convert.ToBoolean(value);
                    break;
                case "string":
                    value = value.ToString();
                    break;
            }

            stepInput[prop.Name] = value;
        }

        return stepInput;
    }

    public static void MapOutputs(string outputMapping, object stepResult, Dictionary<string, object> contextData)
    {
        if (string.IsNullOrEmpty(outputMapping) || stepResult == null) return;

        var mapping = JObject.Parse(outputMapping);
        foreach (var prop in mapping.Properties())
        {
            var destKey = prop.Value.ToString();
            if (stepResult is JObject jObj && jObj.ContainsKey(prop.Name))
                contextData[destKey] = jObj[prop.Name];
            else if (stepResult is Dictionary<string, object> dict && dict.ContainsKey(prop.Name))
                contextData[destKey] = dict[prop.Name];
            else
                contextData[destKey] = stepResult;
        }
    }
}
