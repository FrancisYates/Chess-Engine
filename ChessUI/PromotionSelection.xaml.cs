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

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for PromotionSelection.xaml
    /// </summary>
    public partial class PromotionSelection : Window
    {
        MainWindow mainWindow;

        public PromotionSelection(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void HandelClick(Move.PromotionType promotionType)
        {
            mainWindow.SetPromotion(Move.MoveType.promotion, promotionType);
            this.Close();
        }

        private void QueenClick(object sender, RoutedEventArgs e)
        {
            HandelClick(Move.PromotionType.queen);
        }
        private void RookClick(object sender, RoutedEventArgs e)
        {
            HandelClick(Move.PromotionType.rook);
        }
        private void BishopClick(object sender, RoutedEventArgs e)
        {
            HandelClick(Move.PromotionType.bishop);
        }
        private void KnightClick(object sender, RoutedEventArgs e)
        {
            HandelClick(Move.PromotionType.knight);
        }
    }
}
