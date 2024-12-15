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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Solitaire.Models;
using Solitaire.Services;
using Solitaire.Records;

namespace Solitaire
{
    public partial class MainWindow : Window
    {
        private GameManager _gameManager;

        public MainWindow()
        {
            InitializeComponent();
            _gameManager = new GameManager(GameCanvas);

            // Подписка на клик по колоде
            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(GameCanvas);

            // Проверяем, попал ли клик в область колоды (Stock)
            if (clickPoint.X >= 50 && clickPoint.X <= 150 && clickPoint.Y >= 10 && clickPoint.Y <= 150)
            {
                _gameManager.MoveCardFromStockToWaste();
            }
        }


        private void NewGameButton_Click(object sender, RoutedEventArgs e) => _gameManager.StartNewGame();

        private void SaveGameButton_Click(object sender, RoutedEventArgs e) => _gameManager.SaveGame();

        private void LoadGameButton_Click(object sender, RoutedEventArgs e) => _gameManager.LoadGame();

        private void RestartButton_Click(object sender, RoutedEventArgs e) => _gameManager.RestartGame();

        private void ViewRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            var recordsWindow = new RecordsWindow();
            recordsWindow.ShowDialog();
        }
    }
}
