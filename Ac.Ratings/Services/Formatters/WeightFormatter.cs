using System.Text.RegularExpressions;

namespace Ac.Ratings.Services.Formatters;

public class WeightFormatter : BaseValueFormatter {
    public override string? TransformValue(string? value) {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var weightValue = value.Replace(" ", "").ToLower();
        return ConvertWeightString(weightValue);
    }

    private string ConvertWeightString(string weight) {
        if (string.IsNullOrWhiteSpace(weight)) {
            return "-";
        }

        weight = Regex.Replace(weight, "[^0-9kg]", "");
        var match = Regex.Match(weight, @"(\d+)kg");
        if (match.Success) {
            string value = match.Groups[1].Value;
            return $"{value} kg";
        }

        return "-";
    }
}