using StreamingPlatform.Data;
using StreamingPlatform.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace StreamingPlatform
{
    public partial class LoginWindow : Window
    {
        public User? LoggedInUser { get; private set; }
        private bool _isRegisterMode = false;

        public LoginWindow()
        {
            InitializeComponent();
            SeedData();
        }

        private void SeedData()
        {
            using var db = new ApplicationDbContext();
            db.Database.EnsureCreated();

            // Создаём admin если нет
            if (!db.Users.Any(u => u.Username == "admin"))
            {
                db.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = "admin123",
                    Email = "admin@stream.com",
                    Country = "Germany",
                    RegistrationDate = DateTime.Now
                });
                db.SaveChanges();
            }

            // Тестовые треки
            if (!db.Tracks.Any())
            {
                db.Tracks.AddRange(
                    new Track { Title = "Blinding Lights", Duration = 200 },
                    new Track { Title = "Shape of You", Duration = 234 },
                    new Track { Title = "Levitating", Duration = 203 },
                    new Track { Title = "Stay", Duration = 141 },
                    new Track { Title = "Bad Guy", Duration = 194 },
                    new Track { Title = "Watermelon Sugar", Duration = 174 }
                );
                db.SaveChanges();
            }
        }

        private void TabLogin_Click(object sender, RoutedEventArgs e)
        {
            _isRegisterMode = false;
            TabLogin.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#1DB954"));
            TabLogin.Foreground = Brushes.White;
            TabRegister.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#2a2a2a"));
            TabRegister.Foreground = Brushes.Gray;
            FieldEmail.Visibility = Visibility.Collapsed;
            ActionButton.Content = "Войти";
            HintText.Text = "Нет аккаунта? Нажмите «Регистрация»";
            ErrorText.Text = "";
        }

        private void TabRegister_Click(object sender, RoutedEventArgs e)
        {
            _isRegisterMode = true;
            TabRegister.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#1DB954"));
            TabRegister.Foreground = Brushes.White;
            TabLogin.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#2a2a2a"));
            TabLogin.Foreground = Brushes.Gray;
            FieldEmail.Visibility = Visibility.Visible;
            ActionButton.Content = "Зарегистрироваться";
            HintText.Text = "Уже есть аккаунт? Нажмите «Вход»";
            ErrorText.Text = "";
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRegisterMode) Register();
            else Login();
        }

        private void Login()
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "Введите логин и пароль.";
                return;
            }

            using var db = new ApplicationDbContext();
            var user = db.Users
                .FirstOrDefault(u => u.Username == username
                                  && u.PasswordHash == password);

            if (user == null)
            {
                ErrorText.Text = "Неверный логин или пароль.";
                return;
            }

            LoggedInUser = user;
            DialogResult = true;
            Close();
        }

        private void Register()
        {
            string username = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
            {
                ErrorText.Text = "Заполните все поля.";
                return;
            }

            using var db = new ApplicationDbContext();

            if (db.Users.Any(u => u.Username == username))
            {
                ErrorText.Text = "Такой логин уже занят.";
                return;
            }

            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = password,
                Country = "Unknown",
                RegistrationDate = DateTime.Now
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            // Сразу входим после регистрации
            LoggedInUser = db.Users.First(u => u.Username == username);
            DialogResult = true;
            Close();
        }
    }
}
