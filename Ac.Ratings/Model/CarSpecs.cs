using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ac.Ratings.Core;
using Ac.Ratings.Services.Formatters;

namespace Ac.Ratings.Model;

public class CarSpecs : ObservableObject {
    private readonly string? _folderPath;
    private JsonElement? _specsElement;

    public CarSpecs(string? folderPath) {
        _folderPath = folderPath;
        LoadSpecs();
    }

    public string? Bhp => GetConvertedValue("bhp", new PowerFormatter());
    public string? Torque => GetConvertedValue("torque", new TorqueFormatter());
    public string? Weight => GetConvertedValue("weight", new WeightFormatter());
    public string? TopSpeed => GetConvertedValue("topspeed", new TopSpeedFormatter());
    public string? Acceleration => GetConvertedValue("acceleration", new AccelerationFormatter());
    public string? PowerToWeightRatio => GetRawValue("pwratio"); // No conversion needed

    private string? GetConvertedValue(string key, JsonConverter<string?> converter) {
        var rawValue = GetRawValue(key);
        return rawValue != null ? ((dynamic)converter).TransformValue(rawValue) : "-";
    }

    private void LoadSpecs() {
        if (string.IsNullOrEmpty(_folderPath)) return;

        var uiCarPath = Path.Combine(_folderPath, "ui", "ui_car.json");
        if (!File.Exists(uiCarPath)) return;

        try {
            var json = File.ReadAllText(uiCarPath);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("specs", out var specsFromJson) && specsFromJson.ValueKind == JsonValueKind.Object) {
                _specsElement = specsFromJson.Clone(); // Clone to manage lifecycle independently
            }
            else {
                _specsElement = null;
            }
        }
        catch (JsonException ex) {
            Console.WriteLine($"Error parsing specs JSON in {uiCarPath}: {ex.Message}");
            _specsElement = null;
        }
        catch (IOException ex) {
            Console.WriteLine($"Error reading specs file {uiCarPath}: {ex.Message}");
            _specsElement = null;
        }
        catch (Exception ex) {
            Console.WriteLine($"Unexpected error loading specs from {uiCarPath}: {ex.Message}");
            _specsElement = null;
        }
    }

    private string? GetRawValue(string key) {
        if (!_specsElement.HasValue) return "-"; // Return default if loading failed

        if (_specsElement.Value.TryGetProperty(key, out var valueElement) && valueElement.ValueKind == JsonValueKind.String) {
            return valueElement.GetString();
        }

        return "-"; // Return default if key not found or not a string
    }
}