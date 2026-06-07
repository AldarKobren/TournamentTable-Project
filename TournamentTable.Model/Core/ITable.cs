using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTable.Model.Core
{
    public interface ITable
    {
        string Title { get; set; }
        int SeasonYear { get; set; }

        void Match(Team team1, Team team2);

        void Match(Team team1, Team team2, int score1, int score2);

        void SortDefault();
        void SortByScore();
    }
}
