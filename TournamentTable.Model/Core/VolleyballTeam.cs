using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTable.Model.Core
{
    public class VolleyballTeam : Team
    {
        public VolleyballTeam(string name) : base(name) { }
        public override int CalculatePoints => Wins * 3;
    }
}
