using System.Text.RegularExpressions;

namespace Ac.Ratings.Services.Formatters;

public class TorqueFormatter : BaseValueFormatter {
    public override string? TransformValue(string? value) {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var torqueValue = value.Replace(" ", "").ToLower();
        return ConvertTorqueString(torqueValue);
    }

    private string ConvertTorqueString(string torque) {
        torque = Regex.Replace(torque, "[^0-9nm+]", "");
        var match = Regex.Match(torque, @"(\d+)(\+?)nm");
        if (match.Success) {
            string value = match.Groups[1].Value;
            string hasPlusSymbol = match.Groups[2].Value;
            return $"{value}{hasPlusSymbol} Nm";
        }

        return "-";
    }
}