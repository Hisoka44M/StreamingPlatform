using Microsoft.EntityFrameworkCore;
using StreamingPlatform.Data;
using StreamingPlatform.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StreamingPlatform
{
    public class TrackViewModel
    {
        public int TrackID { get; set; }
        public string? Title { get; set; }
        public int Duration { get; set; }
        public string DurationFormatted =>
            $"{Duration / 60}:{Duration % 60:D2}";
    }

    public partial class UserWindow : Window
    {
        private readonly User _user;
        private bool _loaded = false; // защита от срабатывания до загрузки UI

        public UserWindow(User user)
        {
            InitializeComponent();
            _user = user;
            _loaded = true;

            WelcomeText.Text = $"👤 {_user.Username}";
            SubText.Text = "Подписка: Free";

            LoadTracks();
            LoadPlaylists();
        }

        private void BtnTracks_Click(object sender, RoutedEventArgs e)
        {
            PanelTracks.Visibility = Visibility.Visible;
            PanelPlaylists.Visibility = Visibility.Collapsed;
            BtnTracks.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#1DB954"));
            BtnTracks.Foreground = Brushes.White;
            BtnPlaylists.Background = Brushes.Transparent;
            BtnPlaylists.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#aaaaaa"));
        }

        private void BtnPlaylists_Click(object sender, RoutedEventArgs e)
        {
            PanelTracks.Visibility = Visibility.Collapsed;
            PanelPlaylists.Visibility = Visibility.Visible;
            BtnPlaylists.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#1DB954"));
            BtnPlaylists.Foreground = Brushes.White;
            BtnTracks.Background = Brushes.Transparent;
            BtnTracks.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#aaaaaa"));
            LoadPlaylists();
        }

        private void LoadTracks(string search = "")
        {
            using var db = new ApplicationDbContext();
            var query = db.Tracks.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Title!.ToLower().Contains(search.ToLower()));

            TracksList.ItemsSource = query.Select(t => new TrackViewModel
            {
                TrackID = t.TrackID,
                Title = t.Title,
                Duration = t.Duration ?? 0
            }).ToList();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск треков...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.White;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchBox.Text = "Поиск треков...";
                SearchBox.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#aaaaaa"));
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Защита: не выполнять до того как UI полностью загружен
            if (!_loaded) return;

            string text = SearchBox.Text == "Поиск треков..." ? "" : SearchBox.Text;
            LoadTracks(text);
        }

        private void LoadPlaylists()
        {
            using var db = new ApplicationDbContext();
            var playlists = db.Playlists
                .Where(p => p.UserID == _user.UserID)
                .ToList();

            PlaylistsList.ItemsSource = playlists;
            PlaylistPicker.ItemsSource = playlists;
        }

        private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string name = NewPlaylistName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название плейлиста.");
                return;
            }

            using var db = new ApplicationDbContext();
            db.Playlists.Add(new Playlist
            {
                PlaylistName = name,
                UserID = _user.UserID,
                CreationDate = DateTime.Now
            });
            db.SaveChanges();

            NewPlaylistName.Text = "";
            LoadPlaylists();
            MessageBox.Show($"Плейлист «{name}» создан!");
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (TracksList.SelectedItem is not TrackViewModel selectedTrack)
            {
                MessageBox.Show("Выберите трек из списка.");
                return;
            }

            if (PlaylistPicker.SelectedItem is not Playlist selectedPlaylist)
            {
                MessageBox.Show("Выберите плейлист.");
                return;
            }

            using var db = new ApplicationDbContext();

            bool already = db.PlaylistTracks.Any(pt =>
                pt.PlaylistID == selectedPlaylist.PlaylistID &&
                pt.TrackID == selectedTrack.TrackID);

            if (already)
            {
                MessageBox.Show("Этот трек уже есть в плейлисте.");
                return;
            }

            db.PlaylistTracks.Add(new PlaylistTrack
            {
                PlaylistID = selectedPlaylist.PlaylistID,
                TrackID = selectedTrack.TrackID
            });
            db.SaveChanges();

            MessageBox.Show($"«{selectedTrack.Title}» добавлен в «{selectedPlaylist.PlaylistName}»!");

            if (PlaylistsList.SelectedItem is Playlist current &&
                current.PlaylistID == selectedPlaylist.PlaylistID)
                ShowPlaylistTracks(selectedPlaylist);
        }

        private void PlaylistsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistsList.SelectedItem is Playlist playlist)
                ShowPlaylistTracks(playlist);
        }

        private void ShowPlaylistTracks(Playlist playlist)
        {
            using var db = new ApplicationDbContext();
            var tracks = db.PlaylistTracks
                .Where(pt => pt.PlaylistID == playlist.PlaylistID)
                .Include(pt => pt.Track)
                .Select(pt => new TrackViewModel
                {
                    TrackID = pt.Track!.TrackID,
                    Title = pt.Track.Title,
                    Duration = pt.Track.Duration ?? 0
                })
                .ToList();

            PlaylistTracksList.ItemsSource = tracks;
            PlaylistTracksTitle.Text = $"Треки плейлиста «{playlist.PlaylistName}» ({tracks.Count})";
        }
    }
}