using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TournamentTable.Model.Data
{
    public class JsonTournamentSerializer<T> : BaseSerializer<T>
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto, 
            Formatting = Newtonsoft.Json.Formatting.Indented
        };

        public override void Serialize(string filePath, T data)
        {
            try
            {
                EnsureDirectoryExists(filePath);
                string json = JsonConvert.SerializeObject(data, _settings);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при сохранении данных в JSON: {ex.Message}", ex);
            }
        }

        public override T Deserialize(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Файл не найден: {filePath}");

                string json = File.ReadAllText(filePath);
                T data = JsonConvert.DeserializeObject<T>(json, _settings);

                if (data == null)
                    throw new InvalidDataException("Файл JSON пуст или поврежден.");

                return data;
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при чтении данных из JSON: {ex.Message}", ex);
            }
        }
    }
}
