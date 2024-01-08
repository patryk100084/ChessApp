using ChessApp.Game;
using ChessApp.Utilities.Database;
using ChessApp.Utilities;
using System;
using System.Windows;

namespace ChessApp.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string username = "";
        string password = "";
        public Player player;
        void LoginUser(object sender, RoutedEventArgs e)
        {
            PlayerService service = new PlayerService(new PlayerContext());
            username = usernameText.Text;
            password = passwordText.Password;
            if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
                MessageBox.Show("Fill in all input in form to login", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                try
                {
                    string hashedPassword = StringHasher.Hash(password);
                    player = service.GetPlayer(username, hashedPassword);
                    Close();
                }
                catch
                {
                   MessageBox.Show("Error occured while loggin in - make sure specified user exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public LoginWindow()
        {
            InitializeComponent();
        }
    }
}
