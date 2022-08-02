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

        private void HandelClick(int selectedPiece)
        {
            switch (selectedPiece)
            {
                case 0:
                    mainWindow.SetPromotion(Move.MoveType.promotionQueen);
                    break;
                case 1:
                    mainWindow.SetPromotion(Move.MoveType.promotionRook);
                    break;
                case 2:
                    mainWindow.SetPromotion(Move.MoveType.promotionBishop);
                    break;
                case 3:
                    mainWindow.SetPromotion(Move.MoveType.promotionKnight);
                    break;
            }
            this.Close();
        }

        private void QueenClick(object sender, RoutedEventArgs e)
        {
            HandelClick(0);
        }
        private void RookClick(object sender, RoutedEventArgs e)
        {
            HandelClick(1);
        }
        private void BishopClick(object sender, RoutedEventArgs e)
        {
            HandelClick(2);
        }
        private void KnightClick(object sender, RoutedEventArgs e)
        {
            HandelClick(3);
        }
    }
}
