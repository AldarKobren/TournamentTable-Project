using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTable.Model.Core
{
    public class BasketballTeam : Team
    {
        public BasketballTeam(string name) : base(name) { }
        public override int CalculatePoints => (Wins * 2) + Losses;
    }
}
