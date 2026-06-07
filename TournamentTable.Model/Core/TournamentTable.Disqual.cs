using System;
using System.Linq;
using TournamentTable.Model.Core;

namespace TournamentTable.Model.Core
{
    public partial class TournamentTable<T> : ITable where T : Team
    {
        public void Disqual(Team team)
        {
            if (Teams == null || team == null) return;

            var list = Teams.ToList();
            var disqualifiedList = DisqualifiedTeams.ToList();

            var target = list.FirstOrDefault(t => t.Name == team.Name);

            if (target != null)
            {
                list.Remove(target);
                disqualifiedList.Add((T)target);

                Teams = list.ToArray();
                DisqualifiedTeams = disqualifiedList.ToArray();

                SortByScore();
            }
        }

        public void Restore(Team team)
        {
            if (DisqualifiedTeams == null || team == null) return;

            var list = Teams.ToList();
            var disqualifiedList = DisqualifiedTeams.ToList();

            var target = disqualifiedList.FirstOrDefault(t => t.Name == team.Name);

            if (target != null)
            {
                disqualifiedList.Remove(target);
                list.Add((T)target);

                Teams = list.ToArray();
                DisqualifiedTeams = disqualifiedList.ToArray();

                SortByScore();
            }
        }
    }
}