using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TournamentTable.Model.Core;
using TournamentTable.Model.Data;

namespace TournamentTable
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSportType != null && cbSeasonYear != null)
            {
                btnShowTable.IsEnabled = (cbSportType.SelectedIndex != -1 && cbSeasonYear.SelectedIndex != -1);
            }
        }

        private void BtnShowTable_Click(object sender, RoutedEventArgs e)
        {
            string sport = ((ComboBoxItem)cbSportType.SelectedItem).Content.ToString();
            int year = int.Parse(((ComboBoxItem)cbSeasonYear.SelectedItem).Content.ToString());

            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TournamentData");
            string filePath = Path.Combine(folderPath, $"{sport}_{year}.json");

            try
            {
                if (!File.Exists(filePath))
                {
                    GenerateDefaultJsonData(sport, year, filePath);
                }

                TournamentWindow tableWindow = new TournamentWindow(filePath, sport, year);

                tableWindow.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подготовке или открытии данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateDefaultJsonData(string sport, int year, string filePath)
        {
            string title = $"{sport} - Сезон {year}";

            if (sport == "Футбол")
            {
                var teams = new FootballTeam[]
                {
                    new FootballTeam("Зенит"), new FootballTeam("Спартак"),
                    new FootballTeam("ЦСКА"), new FootballTeam("Локомотив"),
                    new FootballTeam("Краснодар"), new FootballTeam("Динамо")
                };
                var tournament = new TournamentTable<FootballTeam>(title, year, teams);

                Random r = new Random();
                tournament.Match(teams[0], teams[1], r.Next(0, 4), r.Next(0, 4));
                tournament.Match(teams[2], teams[3], r.Next(0, 4), r.Next(0, 4));

                tournament.SortDefault(); 

                var serializer = new JsonTournamentSerializer<TournamentTable<FootballTeam>>();
                serializer.Serialize(filePath, tournament);
            }
            else if (sport == "Баскетбол")
            {
                var teams = new BasketballTeam[]
                {
                    new BasketballTeam("УНИКС"), new BasketballTeam("ЦСКА Москва"),
                    new BasketballTeam("Зенит Баскет"), new BasketballTeam("Локомотив-Кубань")
                };
                var tournament = new TournamentTable<BasketballTeam>(title, year, teams);

                tournament.SortDefault();

                var serializer = new JsonTournamentSerializer<TournamentTable<BasketballTeam>>();
                serializer.Serialize(filePath, tournament);
            }
            else
            {
                var teams = new VolleyballTeam[]
                {
                    new VolleyballTeam("Зенит Казань"), new VolleyballTeam("Динамо Москва"),
                    new VolleyballTeam("Локомотив Новосибирск"), new VolleyballTeam("Белогорье")
                };
                var tournament = new TournamentTable<VolleyballTeam>(title, year, teams);

                tournament.SortDefault();

                var serializer = new JsonTournamentSerializer<TournamentTable<VolleyballTeam>>();
                serializer.Serialize(filePath, tournament);
            }
        }
    }
}