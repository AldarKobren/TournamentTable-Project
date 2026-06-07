using System;
using System.Windows;
using TournamentTable.Model.Core;
using TournamentTable.Model.Data;

namespace TournamentTable
{
    public partial class TournamentWindow : Window
    {
        private readonly string _filePath;
        private readonly string _sport;

        private dynamic _tournament;

        public TournamentWindow(string filePath, string sport, int year)
        {
            InitializeComponent();
            _filePath = filePath;
            _sport = sport;

            txtTournamentTitle.Text = $"{sport} — Сезон {year}";
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (_sport == "Футбол")
                {
                    var serializer = new JsonTournamentSerializer<TournamentTable<FootballTeam>>();
                    _tournament = serializer.Deserialize(_filePath);
                }
                else if (_sport == "Баскетбол")
                {
                    var serializer = new JsonTournamentSerializer<TournamentTable<BasketballTeam>>();
                    _tournament = serializer.Deserialize(_filePath);
                }
                else
                {
                    var serializer = new JsonTournamentSerializer<TournamentTable<VolleyballTeam>>();
                    _tournament = serializer.Deserialize(_filePath);
                }

                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshGrid()
        {
            dgTeams.ItemsSource = null;
            dgTeams.ItemsSource = _tournament.Teams;
        }
        private void BtnSortByName_Click(object sender, RoutedEventArgs e)
        {
            _tournament.SortDefault();
            RefreshGrid();
            SaveData();
        }

        private void BtnSortByScore_Click(object sender, RoutedEventArgs e)
        {
            _tournament.SortByScore();
            RefreshGrid();
            SaveData();
        }

        private void BtnSimulateMatch_Click(object sender, RoutedEventArgs e)
        {
            if (_tournament.Teams.Length < 2) return;

            var team1 = _tournament.Teams[0];
            var team2 = _tournament.Teams[1];

            _tournament.Match(team1, team2);

            MessageBox.Show($"Матч сыгран!\n{team1.Name} против {team2.Name}\nТекущие очки обновлены.",
                            "Матч завершен", MessageBoxButton.OK, MessageBoxImage.Information);

            RefreshGrid();
            SaveData();
        }

        private void SaveData()
        {
            try
            {
                if (_sport == "Футбол")
                {
                    var serializer = new JsonTournamentSerializer<TournamentTable<FootballTeam>>();
                    serializer.Serialize(_filePath, _tournament);
                }
                else if (_sport == "Баскетбол")
                {
                    var serializer = new JsonTournamentSerializer<TournamentTable<BasketballTeam>>();
                    serializer.Serialize(_filePath, _tournament);
                }
                else
                {
                    var serializer = new JsonTournamentSerializer<TournamentTable<VolleyballTeam>>();
                    serializer.Serialize(_filePath, _tournament);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка автосохранения: {ex.Message}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}