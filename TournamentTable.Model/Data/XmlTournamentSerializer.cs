using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TournamentTable.Model.Core;

namespace TournamentTable.Model.Data
{
    public class XmlTournamentSerializer<T> : BaseSerializer<T>
    {
        private readonly XmlSerializer _xmlSerializer;

        public XmlTournamentSerializer()
        {
            Type[] extraTypes = new Type[] { typeof(FootballTeam), typeof(BasketballTeam), typeof(VolleyballTeam) };
            _xmlSerializer = new XmlSerializer(typeof(T), extraTypes);
        }

        public override void Serialize(string filePath, T data)
        {
            try
            {
                EnsureDirectoryExists(filePath);
                using (var writer = new StreamWriter(filePath))
                {
                    _xmlSerializer.Serialize(writer, data);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при сохранении данных в XML: {ex.Message}", ex);
            }
        }

        public override T Deserialize(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Файл не найден: {filePath}");

                using (var reader = new StreamReader(filePath))
                {
                    T data = (T)_xmlSerializer.Deserialize(reader);
                    if (data == null)
                        throw new InvalidDataException("Файл XML пуст или поврежден.");

                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при чтении данных из XML: {ex.Message}", ex);
            }
        }
    }
}
