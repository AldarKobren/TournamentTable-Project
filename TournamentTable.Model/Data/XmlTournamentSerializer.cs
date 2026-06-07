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
        private readonly XmlSerializer _serializer;

        public XmlTournamentSerializer(Type[] extraTypes = null)
        {
            _serializer = new XmlSerializer(typeof(T), extraTypes);
        }

        public override void Serialize(string filePath, T data)
        {
            EnsureDirectoryExists(filePath);
            using (var writer = new StreamWriter(filePath))
            {
                _serializer.Serialize(writer, data);
            }
        }

        public override T Deserialize(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Файл {filePath} не найден.");
            using (var reader = new StreamReader(filePath))
            {
                return (T)_serializer.Deserialize(reader);
            }
        }
    }
}
