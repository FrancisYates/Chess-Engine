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

namespace ChessUI.Views
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new(this);
            gameWindow.Show();
            this.Hide();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            GameSettings gameSettings = new(this);
            gameSettings.Show();
            this.Hide();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
