using System.Text.Json;

using Xunit.Sdk;

[assembly: RegisterXunitSerializer(
    typeof(SatSolverCore.Tests.FormulaSerializer),
    typeof(SatSolverCore.Formula)
)]

namespace SatSolverCore.Tests;

public class FormulaSerializer : IXunitSerializer
{
    public object Deserialize(Type type, string serializedValue)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return JsonSerializer.Deserialize<Formula>(serializedValue);
#pragma warning restore CS8603
    }

    public bool IsSerializable(Type type, object? value, out string failureReason)
    {
        if (type.Equals(typeof(Formula)))
        {
            failureReason = string.Empty;
            return true;
        }

        failureReason = $"Unsupported type: {type}";
        return false;
    }

    public string Serialize(object value)
    {
        return JsonSerializer.Serialize(value);
    }
}
