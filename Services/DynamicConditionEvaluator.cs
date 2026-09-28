using System.Linq.Dynamic.Core;
using System.Security;
using System.Text.RegularExpressions;
namespace ServiceAggregator.Api.Services;
public static class DynamicConditionEvaluator
{
    private const int MaxConditionLength = 300;
    private static readonly string[] ForbiddenKeywords =
    {
        "new ", "typeof", "GetType", "Assembly", "System.",
        "Invoke", "Method", "Activator", "Convert",
        "Console", "File", "Directory"
    };

    private static readonly Regex AllowedCharsRegex =
        new(@"^[a-zA-Z0-9_><=!&|().""\s]+$");
    public static bool Evaluate(
        string condition,
        IDictionary<string, object> variables,
        IEnumerable<string> allowedVariables)
    {
        ValidateCondition(condition);
        ValidateVariables(condition, variables, allowedVariables);

        var parameterType = CreateDynamicType(variables, allowedVariables);
        var parameterValue = CreateDynamicObject(parameterType, variables);

        var queryable = new[] { parameterValue }.AsQueryable();

        var config = new ParsingConfig
        {
            AllowNewToEvaluateAnyType = false,
            ResolveTypesBySimpleName = false
        };

        return queryable.Where(config, condition).Any();
    }

    private static void ValidateCondition(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            throw new SecurityException("شرط خالی است");

        if (condition.Length > MaxConditionLength)
            throw new SecurityException("شرط خیلی طولانی است");

        if (!AllowedCharsRegex.IsMatch(condition))
            throw new SecurityException("شرط دارای کاراکتر های غیر قانونی میباشد");

        foreach (var keyword in ForbiddenKeywords)
        {
            if (condition.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                throw new SecurityException($"کلمه غیر قانونی: {keyword}");
        }
    }

    private static void ValidateVariables(
        string condition,
        IDictionary<string, object> variables,
        IEnumerable<string> allowedVariables)
    {
        foreach (var variable in variables.Keys)
        {
            if (!allowedVariables.Contains(variable))
                throw new SecurityException($" متغییر '{variable}' اجازه استفاده ندارد");

            if (!condition.Contains(variable))
                continue;
        }
    }

    private static Type ResolveClrType(object value)
    {
        if (value is null)
            return typeof(object);

        return value switch
        {
            int => typeof(int),
            long => typeof(long),
            decimal => typeof(decimal),
            double => typeof(double),
            bool => typeof(bool),
            string => typeof(string),
            DateTime => typeof(DateTime),
            _ => throw new NotSupportedException(
                $"Type {value.GetType().Name} ساپورت نمیشود")
        };
    }

    private static Type CreateDynamicType(
        IDictionary<string, object> variables,
        IEnumerable<string> allowedVariables)
    {
        var fields = variables
            .Where(v => allowedVariables.Contains(v.Key))
            .Select(v => new DynamicProperty(
                v.Key,
                ResolveClrType(v.Value)
            ))
            .ToArray();

        return DynamicClassFactory.CreateType(fields);
    }

    private static object CreateDynamicObject(
        Type type,
        IDictionary<string, object> variables)
    {
        var obj = Activator.CreateInstance(type)!;

        foreach (var kv in variables)
        {
            var prop = type.GetProperty(kv.Key);
            if (prop == null) continue;

            var converted = Convert.ChangeType(kv.Value, prop.PropertyType);
            prop.SetValue(obj, converted);
        }

        return obj;
    }
}
