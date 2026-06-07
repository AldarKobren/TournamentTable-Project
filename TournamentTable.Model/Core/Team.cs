using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTable.Model.Core
{
    public abstract class Team
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException("Название команды не может быть пустым.");
                }
            }
        }

        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int MatchesPlayed => Wins + Draws + Losses;
        public abstract int CalculatePoints { get; }
        protected Team(string name)
        {
            Name = name;
        }
        public override string ToString()
        {
            return $"{Name} (Очки: {CalculatePoints}, В: {Wins}, Н: {Draws} П: {Losses})";
        }
        public static bool operator >(Team t1, Team t2)
        {
            if (t1.CalculatePoints != t2.CalculatePoints)
                return t1.CalculatePoints > t2.CalculatePoints;

            if (t1.Wins != t2.Wins)
                return t1.Wins > t2.Wins;

            return t1.Draws > t2.Draws;
        }

        public static bool operator <(Team t1, Team t2)
        {
            if (t1.CalculatePoints != t2.CalculatePoints)
                return t1.CalculatePoints < t2.CalculatePoints;

            if (t1.Wins != t2.Wins)
                return t1.Wins < t2.Wins;

            return t1.Draws < t2.Draws;
        }

        public static bool operator >=(Team t1, Team t2) => !(t1 < t2);
        public static bool operator <=(Team t1, Team t2) => !(t1 > t2);
    }
}
