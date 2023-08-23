using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for GameSettings.xaml
    /// </summary>
    public partial class GameSettings : Window
    {
        private readonly Menu _menu;
        public GameSettings(Menu menu)
        {
            InitializeComponent();
            _menu = menu;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void WhiteTimeChanged(object sender, TextChangedEventArgs e)
        {
            GameSetup.TimeControl.WhiteInitialTimeMs = Int32.Parse(WhiteTime.Text) * 1000;
        }
        private void BlackTimeChanged(object sender, TextChangedEventArgs e)
        {
            GameSetup.TimeControl.BlackInitialTimeMs = Int32.Parse(BlackTime.Text) * 1000;
        }
        private void WhiteIncrementChanged(object sender, TextChangedEventArgs e)
        {
            GameSetup.TimeControl.WhiteIncrementMs = Int32.Parse(WhiteIncrement.Text) * 1000;
        }
        private void BlackIncrementChanged(object sender, TextChangedEventArgs e)
        {
            GameSetup.TimeControl.BlackIncrementMs = Int32.Parse(BlackIncrement.Text) * 1000;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            _menu.Show();
            this.Close();
        }
    }
}
