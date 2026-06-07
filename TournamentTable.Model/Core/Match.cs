using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTable.Model.Core
{
    public class Match
    {
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public DateTime MatchDate { get; set; }

        public Match() { }

        public Match(string team1Name, string team2Name, int score1, int score2)
        {
            Team1Name = team1Name;
            Team2Name = team2Name;
            Score1 = score1;
            Score2 = score2;
            MatchDate = DateTime.Now;
        }
    }
}
