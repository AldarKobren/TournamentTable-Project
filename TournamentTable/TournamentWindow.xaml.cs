using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TournamentTable.Model.Core;
using TournamentTable.Model.Data;

namespace TournamentTable
{
    public partial class TournamentWindow : Window
    {
        private readonly string _filePath;
        private readonly string _sport;
        private readonly int _year;
        private dynamic _tournament;

        public TournamentWindow(string filePath, string sport, int year)
        {
            InitializeComponent();

            _filePath = filePath;
            _sport = sport;
            _year = year;

            txtTournamentTitle.Text = $"{sport} — Сезон {year}";

            LoadData();

            // Привязываем выпадающие списки выбора команд для матча к исходному списку команд
            cbTeam1.ItemsSource = _tournament.Teams;
            cbTeam2.ItemsSource = _tournament.Teams;
        }

        private void LoadData()
        {
            try
            {
                if (_sport == "Футбол")
                    _tournament = new JsonTournamentSerializer<TournamentTable<FootballTeam>>().Deserialize(_filePath);
                else if (_sport == "Баскетбол")
                    _tournament = new JsonTournamentSerializer<TournamentTable<BasketballTeam>>().Deserialize(_filePath);
                else
                    _tournament = new JsonTournamentSerializer<TournamentTable<VolleyballTeam>>().Deserialize(_filePath);

                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveData()
        {
            try
            {
                if (_sport == "Футбол")
                    new JsonTournamentSerializer<TournamentTable<FootballTeam>>().Serialize(_filePath, _tournament);
                else if (_sport == "Баскетбол")
                    new JsonTournamentSerializer<TournamentTable<BasketballTeam>>().Serialize(_filePath, _tournament);
                else
                    new JsonTournamentSerializer<TournamentTable<VolleyballTeam>>().Serialize(_filePath, _tournament);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Ключевое изменение: формируем данные для правильного отображения № и Очков
        private void RefreshGrid()
        {
            dgTeams.ItemsSource = null;

            if (_tournament != null && _tournament.Teams != null)
            {
                try
                {
                    // Получаем словарь мест от модели: Dictionary<string, int>, где ключ — имя команды
                    Dictionary<string, int> positions = _tournament.GetTeamsPositions();

                    // Приводим dynamic-коллекцию к IEnumerable, чтобы работал LINQ
                    var teamsList = (_tournament.Teams as IEnumerable)?.Cast<dynamic>();

                    if (teamsList != null)
                    {
                        // Трансформируем элементы в анонимные объекты, свойства которых строго совпадают с Binding в XAML
                        var displayItems = teamsList.Select(team => new
                        {
                            Position = positions.ContainsKey(team.Name) ? positions[team.Name].ToString() : "-",
                            Name = team.Name,
                            MatchesPlayed = team.MatchesPlayed,
                            Wins = team.Wins,
                            Draws = team.Draws,
                            Losses = team.Losses,
                            Points = team.CalculatePoints // Подтягиваем рассчитанные очки
                        }).ToList();

                        dgTeams.ItemsSource = displayItems;
                    }
                }
                catch
                {
                    // Резервный вариант: если GetTeamsPositions еще не реализован в модели, выводим как есть
                    dgTeams.ItemsSource = _tournament.Teams;
                }
            }

            // Обновляем таблицу дисквалифицированных команд
            if (this.FindName("dgDisqualified") is DataGrid dg)
            {
                dg.ItemsSource = null;
                dg.ItemsSource = _tournament.DisqualifiedTeams;
            }
        }

        private void BtnSaveMatch_Click(object sender, RoutedEventArgs e)
        {
            var t1 = cbTeam1.SelectedItem as Team;
            var t2 = cbTeam2.SelectedItem as Team;

            if (t1 == null || t2 == null || t1 == t2)
            {
                MessageBox.Show("Выберите две разные команды!");
                return;
            }

            if (int.TryParse(txtScore1.Text, out int s1) && int.TryParse(txtScore2.Text, out int s2))
            {
                _tournament.Match(t1, t2, s1, s2);
                RefreshGrid();
                SaveData();
            }
            else
            {
                MessageBox.Show("Введите корректный счет!");
            }
        }

        private void BtnDisqualify_Click(object sender, RoutedEventArgs e)
        {
            // Так как в ItemsSource теперь анонимные объекты, dgTeams.SelectedItem возвращает object.
            // Используем динамическое приведение, чтобы вытащить свойство Name и найти команду.
            if (dgTeams.SelectedItem is var selectedObj && selectedObj != null)
            {
                string selectedName = ((dynamic)selectedObj).Name;

                // Ищем реальный объект команды в списке турнира по имени
                var teamsList = (_tournament.Teams as IEnumerable)?.Cast<dynamic>();
                var selectedTeam = teamsList?.FirstOrDefault(t => t.Name == selectedName);

                if (selectedTeam != null)
                {
                    _tournament.Disqual(selectedTeam);
                    RefreshGrid();
                    SaveData();
                    return;
                }
            }

            MessageBox.Show("Выберите команду в основной таблице!");
        }

        private void BtnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (this.FindName("dgDisqualified") is DataGrid dg && dg.SelectedItem is Team selected)
            {
                _tournament.Restore(selected);
                RefreshGrid();
                SaveData();
            }
            else
            {
                MessageBox.Show("Выберите команду в списке дисквалифицированных!");
            }
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

    }
}