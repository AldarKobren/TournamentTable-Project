using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTable.Model.Data
{
    public interface IDataSerializer<T>
    {
        void Serialize(string filePath, T data);
        T Deserialize(string filePath);
    }
}
