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
                EnsureAllDataGenerated(folderPath);

                TournamentWindow tableWindow = new TournamentWindow(filePath, sport, year);
                tableWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подготовке или открытии данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void EnsureAllDataGenerated(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            int[] years = { 2024, 2025, 2026 };
            Random rand = new Random();

            string[] footballNames = { "Зенит", "Спартак", "ЦСКА", "Локомотив", "Краснодар", "Динамо", "ГазМяс" };
            string[] basketballNames = { "УНИКС", "ЦСКА Москва", "Зенит Баскет", "Локомотив-Кубань", "Парма", "ГазМяс" };
            string[] volleyballNames = { "Зенит Казань", "Динамо Москва", "Локомотив Новосиб", "Белогорье", "Факел", "ГазМяс" };

            foreach (int year in years)
            {
                //ФУТБОЛ
                string fbPath = Path.Combine(folderPath, $"Футбол_{year}.json");
                if (!File.Exists(fbPath))
                {
                    var teams = new FootballTeam[footballNames.Length];
                    for (int i = 0; i < footballNames.Length; i++) teams[i] = new FootballTeam(footballNames[i]);

                    var tournament = new TournamentTable<FootballTeam>($"ЧР по футболу - {year}", year, teams);
                    AutoPlayRoundRobin(tournament, teams, rand, isDrawAllowed: true);

                    var serializer = new JsonTournamentSerializer<TournamentTable<FootballTeam>>();
                    serializer.Serialize(fbPath, tournament);
                }

                //БАСКЕТБОЛ
                string bbPath = Path.Combine(folderPath, $"Баскетбол_{year}.json");
                if (!File.Exists(bbPath))
                {
                    var teams = new BasketballTeam[basketballNames.Length];
                    for (int i = 0; i < basketballNames.Length; i++) teams[i] = new BasketballTeam(basketballNames[i]);

                    var tournament = new TournamentTable<BasketballTeam>($"Единая лига ВТБ - {year}", year, teams);
                    AutoPlayRoundRobin(tournament, teams, rand, isDrawAllowed: false); 

                    var serializer = new JsonTournamentSerializer<TournamentTable<BasketballTeam>>();
                    serializer.Serialize(bbPath, tournament);
                }

                //ВОЛЕЙБОЛ
                string vbPath = Path.Combine(folderPath, $"Волейбол_{year}.json");
                if (!File.Exists(vbPath))
                {
                    var teams = new VolleyballTeam[volleyballNames.Length];
                    for (int i = 0; i < volleyballNames.Length; i++) teams[i] = new VolleyballTeam(volleyballNames[i]);

                    var tournament = new TournamentTable<VolleyballTeam>($"Суперлига по волейболу - {year}", year, teams);
                    AutoPlayRoundRobin(tournament, teams, rand, isDrawAllowed: false);

                    var serializer = new JsonTournamentSerializer<TournamentTable<VolleyballTeam>>();
                    serializer.Serialize(vbPath, tournament);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void AutoPlayRoundRobin<T>(TournamentTable<T> tournament, T[] teams, Random rand, bool isDrawAllowed) where T : Team
        {
            for (int i = 0; i < teams.Length; i++)
            {
                for (int j = i + 1; j < teams.Length; j++)
                {
                    int score1 = rand.Next(0, 4);
                    int score2 = rand.Next(0, 4);

                    if (!isDrawAllowed && score1 == score2)
                    {
                        score1 += rand.Next(1, 3);
                    }

                    tournament.Match(teams[i], teams[j], score1, score2);
                }
            }

            tournament.SortDefault();
        }
    }
}