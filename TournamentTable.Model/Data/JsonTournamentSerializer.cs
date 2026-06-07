using System.IO;

namespace TournamentTable.Model.Data
{
    public class JsonTournamentSerializer<T> : BaseSerializer<T>
    {
        private readonly System.Text.Json.JsonSerializerOptions _options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        };

        public override void Serialize(string filePath, T data)
        {
            EnsureDirectoryExists(filePath);

            string json = System.Text.Json.JsonSerializer.Serialize(data, _options);

            File.WriteAllText(filePath, json);
        }

        public override T Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл {filePath} не найден.");

            string json = File.ReadAllText(filePath);

            return System.Text.Json.JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}