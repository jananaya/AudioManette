using System;
using System.Diagnostics;
using System.Windows;
using System.Configuration;
using System.Collections.Specialized;

using AudioManette.AudioSource;
using AudioManette.Authorization;
using AudioManette.Views;

namespace AudioManette
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            OAuthRedirectServer server = new OAuthRedirectServer(5000);

            string? clientId = ConfigurationManager.AppSettings.Get("clientId");
            string? clientSecret = ConfigurationManager.AppSettings.Get("clientSecret");

            if (clientId is null || clientId.Equals("") ||
                clientSecret is null || clientSecret.Equals(""))
            {
                MessageBox.Show("Error, you must assign all the properties in App.config!");
                throw new Exception();
            }

            AuthotizationConfig config = new AuthotizationConfig(
                clientId,
                clientSecret,
                new Uri("http://localhost:5000/")
            );

            string loginUrl = SpotifyAudioSource.GetLoginUrl(config);

            server.Listen();

            var psi = new ProcessStartInfo
            {
                FileName = loginUrl,
                UseShellExecute = true
            };

            Process.Start(psi);

            string code = server.GetCode();
            config.Code = code;

            var spotify = SpotifyAudioSource.CreateAsync(config).GetAwaiter().GetResult();

            var mainWindow = new MainWindow(spotify);

            mainWindow.Show();
        }
    }
}