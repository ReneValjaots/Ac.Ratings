using System.IO;
using Ac.Ratings.Model;
using Ac.Ratings.Services;
using Microsoft.Data.Sqlite;

namespace Ac.Ratings.Data {
    public class EngineDatabaseService {
        private readonly string _dbPath;

        public EngineDatabaseService(string? dbPath = null) {
            _dbPath = dbPath ?? Path.Combine(ConfigManager.DataFolder, "engines.db");
            InitializeDatabase();
        }

        private void InitializeDatabase() {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS CarEngines (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FolderName TEXT NOT NULL UNIQUE,
                Displacement INTEGER,
                Layout TEXT,
                CylinderCount INTEGER
            )";
            command.ExecuteNonQuery();
        }

        public void InsertEngineData(CarEngine engine) {
            using var connection = new SqliteConnection($"Data source={_dbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT OR IGNORE INTO CarEngines (FolderName, Displacement, Layout, CylinderCount) VALUES (@FolderName, @Displacement, @Layout, @CylinderCount)";
            command.Parameters.AddWithValue("@FolderName", engine.FolderName);
            command.Parameters.AddWithValue("@Displacement", engine.Displacement);
            command.Parameters.AddWithValue("@Layout", (object?)engine.Layout ?? DBNull.Value);
            command.Parameters.AddWithValue("@CylinderCount", engine.CylinderCount);
            command.ExecuteNonQuery();
        }

        public Dictionary<string, CarEngine> GetAllEngineData() {
            var engineData = new Dictionary<string, CarEngine>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM CarEngines";
            using var reader = command.ExecuteReader();
            while (reader.Read()) {
                var engine = new CarEngine {
                    Id = reader.GetInt32(0),
                    FolderName = reader.GetString(1),
                    Displacement = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    Layout = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CylinderCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                };
                engineData[engine.FolderName] = engine;
            }
            return engineData;
        }
    }
}
