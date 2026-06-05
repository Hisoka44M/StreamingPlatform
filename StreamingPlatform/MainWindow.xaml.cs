using StreamingPlatform.Data;
using StreamingPlatform.Models;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace StreamingPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> countries = new()
        {
            "Japan",
            "Germany",
            "France",
            "Italy",
            "Spain",
            "Poland",
            "Russia",
            "China",
            "USA",
            "Canada",
            "United Kingdom"
        };

        private void MainGrid_AutoGeneratingColumn(
    object sender,
    DataGridAutoGeneratingColumnEventArgs e)
        {
            // Скрываем навигационное свойство User
            if (e.PropertyName == "User")
            {
                e.Cancel = true;
                return;
            }

            // Делаем Country выпадающим списком
            if (currentTable == "Users" &&
                e.PropertyName == "Country")
            {
                var comboColumn = new DataGridComboBoxColumn
                {
                    Header = "Country",
                    SelectedItemBinding = new Binding("Country"),
                    ItemsSource = countries
                };

                e.Column = comboColumn;
            }
        }

        ApplicationDbContext db = new ApplicationDbContext();
        private List<User> users;
        private List<Track> tracks;
        private List<Playlist> playlists;

        private string currentTable = "Users";

        public MainWindow()
        {
            InitializeComponent();

            db.Database.EnsureCreated();

            LoadUsers();
        }

        private void MainGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            if (currentTable == "Users")
            {
                e.NewItem = new User()
                {
                    Username = "",
                    Email = "",
                    PasswordHash = "",
                    Country = ""
                };
            }

            if (currentTable == "Tracks")
            {
                e.NewItem = new Track()
                {
                    Title = "",
                    Duration = 0
                };
            }

            if (currentTable == "Playlists")
            {
                var firstUser = db.Users.FirstOrDefault();

                e.NewItem = new Playlist()
                {
                    PlaylistName = "",
                    UserID = firstUser?.UserID ?? 0
                };
            }
        }

        // USERS
        private void LoadUsers()
        {
            currentTable = "Users";
            MainGrid.Columns.Clear();

            users = db.Users.ToList();

            MainGrid.ItemsSource = users;
        }

        // TRACKS
        private void LoadTracks()
        {
            currentTable = "Tracks";

            tracks = db.Tracks.ToList();

            MainGrid.ItemsSource = tracks;
        }

        // PLAYLISTS
        private void LoadPlaylists()
        {
            currentTable = "Playlists";

            playlists = db.Playlists.ToList();

            MainGrid.ItemsSource = playlists;
        }





        // КНОПКИ МЕНЮ
        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void TracksButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTracks();
        }

        private void PlaylistsButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPlaylists();
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var statistics = new[]
            {
        new
        {
            Показатель = "Пользователи",
            Значение = db.Users.Count()
        },
        new
        {
            Показатель = "Треки",
            Значение = db.Tracks.Count()
        },
        new
        {
            Показатель = "Плейлисты",
            Значение = db.Playlists.Count()
        }
    };

            MainGrid.ItemsSource = statistics;
        }


        private void CountriesButton_Click(object sender, RoutedEventArgs e)
        {
            var countries = db.Users
                .GroupBy(u => u.Country)
                .Select(g => new
                {
                    Страна = g.Key,
                    Пользователей = g.Count()
                })
                .OrderByDescending(x => x.Пользователей)
                .ToList();

            MainGrid.ItemsSource = countries;
        }



        // ПОИСК
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = (SearchBox.Text ?? "").ToLower();

            if (currentTable == "Users")
            {
                MainGrid.ItemsSource = db.Users
                    .Where(x => (x.Username ?? "")
                    .ToLower()
                    .Contains(search))
                    .ToList();
            }

            if (currentTable == "Tracks")
            {
                MainGrid.ItemsSource = db.Tracks
                    .Where(x => (x.Title ?? "")
                    .ToLower()
                    .Contains(search))
                    .ToList();
            }

            if (currentTable == "Playlists")
            {
                MainGrid.ItemsSource = db.Playlists
                    .Where(x => (x.PlaylistName ?? "")
                    .ToLower()
                    .Contains(search))
                    .ToList();
            }
        }

        // ДОБАВЛЕНИЕ
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentTable == "Users")
            {
                User user = new User()
                {
                    Username = "New User",
                    Email = "user@mail.com",
                    Country = "Germany",
                    PasswordHash = "123456"
                };

                db.Users.Add(user);
            }

            if (currentTable == "Tracks")
            {
                Track track = new Track()
                {
                    Title = "New Track",
                    Duration = 180,
           
                };

                db.Tracks.Add(track);
            }

            if (currentTable == "Playlists")
            {
                var firstUser = db.Users.FirstOrDefault();

                if (firstUser == null)
                {
                    MessageBox.Show("Сначала создайте пользователя.");
                    return;
                }

                Playlist playlist = new Playlist()
                {
                    PlaylistName = "New Playlist",
                    UserID = firstUser.UserID
                };

                db.Playlists.Add(playlist);
            }

            db.SaveChanges();
            RefreshData();
        }

        // УДАЛЕНИЕ
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainGrid.SelectedItem == null)
                    return;

                if (currentTable == "Users")
                {
                    User user = MainGrid.SelectedItem as User;

                    if (user != null)
                    {
                        //db.Users.Attach(user);
                        db.Users.Remove(user);
                    }
                }

                if (currentTable == "Tracks")
                {
                    Track track = MainGrid.SelectedItem as Track;

                    if (track != null)
                    {
                        //db.Tracks.Attach(track);
                        db.Tracks.Remove(track);
                    }
                }

                if (currentTable == "Playlists")
                {
                    Playlist playlist = MainGrid.SelectedItem as Playlist;

                    if (playlist != null)
                    {
                        //db.Playlists.Attach(playlist);
                        db.Playlists.Remove(playlist);
                    }
                }

                db.SaveChanges();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ОБНОВЛЕНИЕ
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на одинаковые Username
                bool duplicateUsers = db.Users
                    .GroupBy(x => x.Username)
                    .Any(g => g.Count() > 1);

                if (duplicateUsers)
                {
                    MessageBox.Show(
                        "Обнаружены пользователи с одинаковым Username.");
                    return;
                }

                // Проверка Email
                foreach (var user in db.Users)
                {
                    if (string.IsNullOrWhiteSpace(user.Email))
                    {
                        MessageBox.Show("Email не может быть пустым.");
                        return;
                    }

                    bool validEmail = Regex.IsMatch(
                        user.Email,
                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                    if (!validEmail)
                    {
                        MessageBox.Show(
                            $"Некорректный email: {user.Email}");
                        return;
                    }
                }

                db.SaveChanges();

                RefreshData();

                MessageBox.Show("Изменения сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshData()
        {
            if (currentTable == "Users")
                LoadUsers();

            if (currentTable == "Tracks")
                LoadTracks();

            if (currentTable == "Playlists")
                LoadPlaylists();

        }
        private void MainGrid_RowEditEnding(
    object sender,
    DataGridRowEditEndingEventArgs e)
        {
            // Убрано намеренно.
            // EF Core сам отслеживает изменения объектов,
            // загруженных из контекста.
        }
    }
}