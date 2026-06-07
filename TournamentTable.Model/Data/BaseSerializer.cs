using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTable.Model.Data
{
    public abstract class BaseSerializer<T>
    {
        public abstract void Serialize(string filePath, T data);
        public abstract T Deserialize(string filePath);

        protected void EnsureDirectoryExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}