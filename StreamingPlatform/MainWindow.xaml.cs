using StreamingPlatform.Data;
using StreamingPlatform.Models;
using System.Linq;
using System.Text;
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
                e.NewItem = new Playlist()
                {
                    PlaylistName = "",
                    UserID = 1
                };
            }
        }

        // USERS
        private void LoadUsers()
        {
            currentTable = "Users";

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

        

        // ПОИСК
        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string search = SearchBox.Text.ToLower();

            if (currentTable == "Users")
            {
                MainGrid.ItemsSource = db.Users
                    .Where(x => x.Username.ToLower().Contains(search))
                    .ToList();
            }

            if (currentTable == "Tracks")
            {
                MainGrid.ItemsSource = db.Tracks
                    .Where(x => x.Title.ToLower().Contains(search))
                    .ToList();
            }

            if (currentTable == "Playlists")
            {
                MainGrid.ItemsSource = db.Playlists
                    .Where(x => x.PlaylistName.ToLower().Contains(search))
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
                    Duration = 180
                };

                db.Tracks.Add(track);
            }

            if (currentTable == "Playlists")
            {
                Playlist playlist = new Playlist()
                {
                    PlaylistName = "New Playlist",
                    UserID = 1
                };

                db.Playlists.Add(playlist);
            }

            db.SaveChanges();
            RefreshData();
        }

        // УДАЛЕНИЕ
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedItem == null)
                return;

            if (currentTable == "Users")
            {
                User user = MainGrid.SelectedItem as User;

                if (user != null)
                {
                    db.Users.Remove(user);
                }
            }

            if (currentTable == "Tracks")
            {
                Track track = MainGrid.SelectedItem as Track;

                if (track != null)
                {
                    db.Tracks.Remove(track);
                }
            }

            if (currentTable == "Playlists")
            {
                Playlist playlist = MainGrid.SelectedItem as Playlist;

                if (playlist != null)
                {
                    db.Playlists.Remove(playlist);
                }
            }

            db.SaveChanges();
            RefreshData();
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
                if (currentTable == "Users")
                {
                    foreach (var user in users)
                    {
                        if (user.UserID == null)
                        {
                            db.Users.Add(user);
                        }
                        else
                        {
                            db.Users.Update(user);
                        }
                    }
                }

                if (currentTable == "Tracks")
                {
                    foreach (var track in tracks)
                    {
                        if (track.TrackID == 0)
                        {
                            db.Tracks.Add(track);
                        }
                        else
                        {
                            db.Tracks.Update(track);
                        }
                    }
                }

                if (currentTable == "Playlists")
                {
                    foreach (var playlist in playlists)
                    {
                        if (playlist.PlaylistID == null)
                        {
                            db.Playlists.Add(playlist);
                        }
                        else
                        {
                            db.Playlists.Update(playlist);
                        }
                    }
                }

                db.SaveChanges();

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
        private void MainGrid_RowEditEnding(object sender,
    DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    if (currentTable == "Users")
                    {
                        User user = e.Row.Item as User;

                        if (user != null &&
                            user.UserID == null)
                        {
                            db.Users.Add(user);
                        }
                    }

                    if (currentTable == "Tracks")
                    {
                        Track track = e.Row.Item as Track;

                        if (track != null &&
                            track.TrackID == 0)
                        {
                            db.Tracks.Add(track);
                        }
                    }

                    if (currentTable == "Playlists")
                    {
                        Playlist playlist = e.Row.Item as Playlist;

                        if (playlist != null &&
                            playlist.PlaylistID == null)
                        {
                            db.Playlists.Add(playlist);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}