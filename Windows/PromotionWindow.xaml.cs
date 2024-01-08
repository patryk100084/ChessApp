using System.Windows;
using System.Windows.Controls;

namespace ChessApp.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy PromotionWindow.xaml
    /// </summary>
    public partial class PromotionWindow : Window
    {
        public string pieceName = "";

        void PromotePawn(object sender, RoutedEventArgs e)
        {
            pieceName = (sender as Button).Content.ToString();
            this.Close();
        }
        public PromotionWindow()
        {
            InitializeComponent();
        }
    }
}
