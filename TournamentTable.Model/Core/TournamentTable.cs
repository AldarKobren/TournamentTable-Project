using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TournamentTable.Model.Core
{
    public partial class TournamentTable<T> : ITable where T : Team
    {
        public string Title { get; set; }
        public int SeasonYear { get; set; }
        public T[] Teams { get; set; } = Array.Empty<T>();
        public Match[] Matches { get; set; } = Array.Empty<Match>();

        public TournamentTable() { }

        public TournamentTable(string title, int seasonYear, T[] teams)
        {
            Title = title;
            SeasonYear = seasonYear;
            Teams = teams;
        }

        // Перегрузка метода Match #1
        public void Match(Team team1, Team team2)
        {
            var rand = new Random();
            Match(team1, team2, rand.Next(0, 4), rand.Next(0, 4));
        }

        // Перегрузка метода Match #2
        public void Match(Team team1, Team team2, int score1, int score2)
        {
            if (team1 == null || team2 == null)
                throw new ArgumentNullException("Команды не могут быть null.");

            var newMatch = new Match(team1.Name, team2.Name, score1, score2);
            var updatedMatches = new List<Match>(Matches) { newMatch };
            Matches = updatedMatches.ToArray();

            if (score1 > score2)
            {
                team1.Wins++;
                team2.Losses++;
            }
            else if (score1 < score2)
            {
                team2.Wins++;
                team1.Losses++;
            }
            else
            {
                team1.Draws++;
                team2.Draws++;
            }
        }

        // Пузырьковая сортировка по алфавиту (SortDefault)
        public void SortDefault()
        {
            for (int i = 0; i < Teams.Length - 1; i++)
            {
                for (int j = 0; j < Teams.Length - i - 1; j++)
                {
                    // Если имя текущей команды по алфавиту дальше, чем следующей — меняем местами
                    if (string.Compare(Teams[j].Name, Teams[j + 1].Name, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        T temp = Teams[j];
                        Teams[j] = Teams[j + 1];
                        Teams[j + 1] = temp;
                    }
                }
            }
        }

        // Пузырьковая сортировка по очкам (SortByScore)
        public void SortByScore()
        {
            for (int i = 0; i < Teams.Length - 1; i++)
            {
                for (int j = 0; j < Teams.Length - i - 1; j++)
                {
                    bool needSwap = false;

                    // 1. Используем наш перегруженный оператор <
                    // Если текущая команда "хуже" следующей, её нужно опустить вниз (поменять местами)
                    if (Teams[j] < Teams[j + 1])
                    {
                        needSwap = true;
                    }
                    // 2. Если по операторам они равны (очки, победы, ничьи совпали), проверяем личную встречу
                    else if (!(Teams[j] > Teams[j + 1]))
                    {
                        var h2h = Matches.FirstOrDefault(m =>
                            (m.Team1Name == Teams[j].Name && m.Team2Name == Teams[j + 1].Name) ||
                            (m.Team1Name == Teams[j + 1].Name && m.Team2Name == Teams[j].Name));

                        if (h2h != null)
                        {
                            if (h2h.Team1Name == Teams[j].Name)
                            {
                                if (h2h.Score1 < h2h.Score2) needSwap = true; // Текущая проиграла личную встречу
                            }
                            else
                            {
                                if (h2h.Score2 < h2h.Score1) needSwap = true; // Текущая проиграла личную встречу
                            }
                        }
                    }

                    if (needSwap)
                    {
                        T temp = Teams[j];
                        Teams[j] = Teams[j + 1];
                        Teams[j + 1] = temp;
                    }
                }
            }
        }

        // Расчет мест с учетом пропусков
        public Dictionary<string, int> GetTeamsPositions()
        {
            var positions = new Dictionary<string, int>();
            if (Teams.Length == 0) return positions;

            SortByScore(); // Сначала сортируем пузырьком

            int currentPlace = 1;
            positions[Teams[0].Name] = currentPlace;

            for (int i = 1; i < Teams.Length; i++)
            {
                T currentTeam = Teams[i];
                T previousTeam = Teams[i - 1];

                // Полное равенство
                bool isFullyEqual = (currentTeam.CalculatePoints == previousTeam.CalculatePoints) &&
                                    (currentTeam.Wins == previousTeam.Wins) &&
                                    (currentTeam.Draws == previousTeam.Draws);

                if (isFullyEqual)
                {
                    var h2h = Matches.FirstOrDefault(m =>
                        (m.Team1Name == currentTeam.Name && m.Team2Name == previousTeam.Name) ||
                        (m.Team1Name == previousTeam.Name && m.Team2Name == currentTeam.Name));

                    if (h2h != null && h2h.Score1 != h2h.Score2) isFullyEqual = false;
                }

                if (isFullyEqual)
                {
                    positions[currentTeam.Name] = positions[previousTeam.Name];
                }
                else
                {
                    positions[currentTeam.Name] = i + 1; // Пропуск места
                }
            }
            return positions;
        }
    }
}
