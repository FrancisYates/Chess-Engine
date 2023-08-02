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
using ChessUI.Enums;

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

        private void HandelClick(PromotionPiece promotionType)
        {
            mainWindow.SetPromotion(MoveType.promotion, promotionType);
            this.Close();
        }

        private void QueenClick(object sender, RoutedEventArgs e)
        {
            HandelClick(PromotionPiece.queen);
        }
        private void RookClick(object sender, RoutedEventArgs e)
        {
            HandelClick(PromotionPiece.rook);
        }
        private void BishopClick(object sender, RoutedEventArgs e)
        {
            HandelClick(PromotionPiece.bishop);
        }
        private void KnightClick(object sender, RoutedEventArgs e)
        {
            HandelClick(PromotionPiece.knight);
        }
    }
}
