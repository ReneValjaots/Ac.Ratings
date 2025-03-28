using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ac.Ratings.Services {
    public static class ConfigManager {
        public static string ResourceFolder { get; private set; }
        public static string DataFolder { get; private set; }
        public static string ConfigFilePath { get; private set; }
        public static string AppearanceConfigFilePath { get; private set; }
        public static string CarsRootFolder { get; private set; }
        public static string ErrorLogFilepath { get; private set; }
        public static string BackupFolder { get; private set; }
        public static string LastUpdatedFilepath {  get; private set; }
        public static string UnpackFolderPath { get; private set; }
        public static string ModifiedRatingsPath { get; private set; }
        public static string? OriginalRatingsPath { get; private set; }
        public static string? AcRootFolder { get; private set; }

        public static readonly JsonSerializerOptions JsonOptions = new() {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        static ConfigManager() {
            ResourceFolder = Path.Combine(AppContext.BaseDirectory, "Resources");
            DataFolder = Path.Combine(AppContext.BaseDirectory, "Data");

            EnsureDirectoryExists(ResourceFolder);
            EnsureDirectoryExists(DataFolder);
            EnsureDirectoryExists(Path.Combine(ResourceFolder, "config"));
            EnsureDirectoryExists(Path.Combine(ResourceFolder, "cars"));
            EnsureDirectoryExists(Path.Combine(ResourceFolder, "data"));
            EnsureDirectoryExists(Path.Combine(ResourceFolder, "backup"));
            EnsureDirectoryExists(Path.Combine(ResourceFolder, "unpackData"));

            ConfigFilePath = Path.Combine(ResourceFolder, "config", "config.json");
            AppearanceConfigFilePath = Path.Combine(ResourceFolder, "config", "appearance.json");
            CarsRootFolder = Path.Combine(ResourceFolder, "cars");
            ErrorLogFilepath = Path.Combine(ResourceFolder, "data", "ErrorLog.txt");
            BackupFolder = Path.Combine(ResourceFolder, "backup");
            LastUpdatedFilepath = Path.Combine(BackupFolder, "LastUpdate.txt");
            UnpackFolderPath = Path.Combine(ResourceFolder, "unpackData");
            ModifiedRatingsPath = Path.Combine(UnpackFolderPath, "Ratings.txt");

            EnsureFileExists(ErrorLogFilepath);
            EnsureFileExists(ConfigFilePath, "{}"); // Ensure config file exists with default empty JSON
            EnsureFileExists(AppearanceConfigFilePath, "{}");

            OriginalRatingsPath = LoadOriginalRatingsPath();
            AcRootFolder = LoadConfigValue("AcRootFolder");
        }

        public static bool EnsureAcRootFolderConfigured(Func<string?> promptUserForPathAction) {
            if (!string.IsNullOrEmpty(AcRootFolder) && Directory.Exists(AcRootFolder)) {
                return true;
            }

            string? userProvidedPath = promptUserForPathAction?.Invoke();

            if (!string.IsNullOrEmpty(userProvidedPath) && Directory.Exists(userProvidedPath)) {
                AcRootFolder = userProvidedPath;
                SaveConfigValue("AcRootFolder", AcRootFolder);
                return true;
            }
            else {
                AcRootFolder = null; 
                return false;
            }
        }

        private static string? LoadConfigValue(string key) {
            return LoadValue(ConfigFilePath, key);
        }

        private static void SaveValue(string filepath, string key, string? value) {
            if (value == null) {
                Console.WriteLine($"Warning: Attempted to save null value for key '{key}' in '{filepath}'. Operation skipped.");
                return;
            }
            Dictionary<string, string> config;
            if (File.Exists(filepath)) {
                config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filepath)) ?? new Dictionary<string, string>();
            }
            else {
                config = new Dictionary<string, string>();
            }

            config[key] = value;
            File.WriteAllText(filepath, JsonSerializer.Serialize(config, JsonOptions));
        }

        private static string? LoadValue(string filepath, string key) {
            if (File.Exists(filepath)) {
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filepath));
                return config?.GetValueOrDefault(key);
            }

            return null;
        }

        private static void SaveConfigValue(string key, string value) {
            SaveValue(ConfigFilePath, key, value);
        }

        public static string? LoadAppearanceConfigValue(string key) {
            return LoadValue(AppearanceConfigFilePath, key);
        }

        public static void SaveAppearanceConfigValue(string key, string value) {
            SaveValue(AppearanceConfigFilePath, key, value);
        }

        private static string? LoadOriginalRatingsPath() {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AcTools Content Manager", "Progress");
            string ratingsPath = Path.Combine(appDataPath, "Ratings.data");

            return File.Exists(ratingsPath) ? ratingsPath : null;
        }

        private static void EnsureDirectoryExists(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        private static void EnsureFileExists(string path, string defaultContent = "") {
            if (!File.Exists(path)) {
                EnsureDirectoryExists(Path.GetDirectoryName(path)!);

                try {
                    File.WriteAllText(path, defaultContent);
                }
                catch (IOException ex) {
                    Console.WriteLine($"Failed to create file {path}: {ex.Message}");
                }
            }
        }
    }
}
