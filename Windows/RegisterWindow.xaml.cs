using System;
using System.Windows;
using ChessApp.Game;
using ChessApp.Utilities.Database;
using ChessApp.Utilities;

namespace ChessApp.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        string username = "";
        string password = "";

        void RegisterUser(object sender, RoutedEventArgs e)
        {
            PlayerService service = new PlayerService(new PlayerContext());
            username = usernameText.Text;
            password = passwordText.Password;

            if(String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
                MessageBox.Show("Fill in all input in form to register user", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                try
                {
                    string hashedPassword = StringHasher.Hash(password);
                    Player newPlayer = new Player { username = username, password = hashedPassword, wins = 0, draws = 0, losses = 0 };
                    service.AddPlayer(newPlayer);
                    MessageBox.Show("User registered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show("Error occured while registering user - check if specified user does not already exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public RegisterWindow()
        {
            InitializeComponent();
        }
    }
}
