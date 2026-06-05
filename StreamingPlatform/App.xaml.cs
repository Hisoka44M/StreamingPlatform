using System.Windows;

namespace StreamingPlatform
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var login = new LoginWindow();

            if (login.ShowDialog() == true && login.LoggedInUser != null)
            {
                var user = login.LoggedInUser;

                if (user.Username == "admin")
                {
                    var main = new MainWindow();
                    main.Closed += (s, args) => Shutdown();
                    main.Show();
                }
                else
                {
                    var userWindow = new UserWindow(user);
                    userWindow.Closed += (s, args) => Shutdown();
                    userWindow.Show();
                }
            }
            else
            {
                Shutdown();
            }
        }
    }
}
