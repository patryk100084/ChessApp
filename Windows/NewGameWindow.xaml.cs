using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessApp.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy NewGameWindow.xaml
    /// </summary>
    public partial class NewGameWindow : Window
    {
        public int gameTime;

        void StartGame(object sender, RoutedEventArgs e)
        {
            gameTime = (int)timeSlider.Value * 600;
            Close();
        }

        void CloseWindow(object sender, RoutedEventArgs e)
        {
            gameTime = 0;
            Close();
        }

        public NewGameWindow()
        {
            InitializeComponent();
        }
    }
}
