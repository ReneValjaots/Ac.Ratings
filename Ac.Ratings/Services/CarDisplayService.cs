using Ac.Ratings.Model;
using Ac.Ratings.Services.Interfaces;

namespace Ac.Ratings.Services {
    public class CarDisplayService : ICarDisplayService {
        private static readonly List<string> _gearboxTags = ["manual", "automatic", "semiautomatic", "sequential"];
        private static readonly List<string> _drivetrainTags = ["rwd", "awd", "fwd"];

        public string ShowCarEngineStats(Car selectedCar) {
            return FormatEngineStats(selectedCar.Engine, selectedCar);
        }

        private static string FormatEngineStats(CarEngine? engine, Car car) {
            if (engine == null || string.IsNullOrEmpty(engine.Layout) || engine.Displacement == 0) {
                return string.Empty;
            }

            var displacementLiters = engine.Displacement > 0 ? (engine.Displacement / 1000.0).ToString("F1") : null;
            var layout = engine.Layout;
            var cylinderCount = engine.CylinderCount > 0 ? engine.CylinderCount.ToString() : null;

            var output = string.Empty;

            if (!string.IsNullOrEmpty(displacementLiters)) output += $"{displacementLiters}l ";
            output = AppendInductionSystemToEngineStats(output, car);

            if (!string.IsNullOrEmpty(layout) && !string.IsNullOrEmpty(cylinderCount)) {
                output += layout switch {
                    "I" => $"inline-{cylinderCount} engine",
                    "V" => $"V{cylinderCount} engine",
                    "B" => $"boxer-{cylinderCount} engine",
                    "R" => "rotary engine",
                    "F" => $"flat-{cylinderCount} engine",
                    _ => string.Empty
                };
            }

            return output.Trim();
        }

        public string ShowCarDriveTrain(Car selectedCar) {
            var tags = selectedCar.Tags;
            var data = selectedCar.Data.TractionType;

            if (data != null) {
                if (data.Contains("rwd", StringComparison.OrdinalIgnoreCase))
                    return "Rear-wheel drive";
                if (data.Contains("awd", StringComparison.OrdinalIgnoreCase))
                    return "All-wheel drive";
                if (data.Contains("fwd", StringComparison.OrdinalIgnoreCase))
                    return "Front-wheel drive";
            }

            var drivetrainFromSpecificTag = tags?.FirstOrDefault(x => x.Contains("#+"))?.Replace(" ", "").ToLower().Remove(0, 2);
            var drivetrainFromRegularTags = tags?.FirstOrDefault(tag => _drivetrainTags.Contains(tag.ToLower()));
            return drivetrainFromSpecificTag?.ToUpper() ?? drivetrainFromRegularTags?.ToUpper() ?? string.Empty;
        }

        public string ShowCarGearbox(Car selectedCar) {
            var gearsCount = selectedCar.Data.GearsCount;
            var isManual = selectedCar.Data.SupportsShifter;
            var tags = selectedCar.Tags;
            var gearboxFromSpecificTag = tags?.FirstOrDefault(x => x.Contains("#-"))?.Replace(" ", "").Remove(0, 2);
            var gearboxFromRegularTags = tags?.FirstOrDefault(tag => _gearboxTags.Contains(tag.ToLower()));

            if (gearsCount == 0)
                return gearboxFromSpecificTag ?? gearboxFromRegularTags ?? string.Empty;

            return isManual switch {
                true => $"{gearsCount}-speed manual transmission",
                false => $"{gearsCount}-speed automatic transmission",
            };
        }

        private static string AppendInductionSystemToEngineStats(string result, Car car) {
            string inductionSystem = ShowInductionSystemForEngineStats(car);
            result += inductionSystem + " ";
            return result;
        }

        private static string ShowInductionSystemForEngineStats(Car car) {
            return car.Data.TurboCount switch {
                1 => "turbocharged",
                2 => "twin turbo",
                _ => "naturally aspirated"
            };
        }
    }
}
