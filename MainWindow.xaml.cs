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
        private UIElement _draggedElement;

        public MainWindow()
        {
            InitializeComponent();
            _gameManager = new GameManager(GameCanvas);
            _gameManager.ScoreUpdated += UpdateScoreUI;
            _gameManager.StockEmpty += OnStockEmpty;

            // Подписка на клик по колоде
            GameCanvas.MouseLeftButtonDown += GameCanvas_MouseLeftButtonDown;

        }
        private void OnStockEmpty()
        {
            // Когда Stock пуст, делаем кнопку перезапуска активной
            RestartButton.IsEnabled = true;
        }

        private void UpdateScoreUI(int score)
        {
            ScoreTextBlock.Text = $"Очки: {score}";
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
        private void GameCanvas_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move; // Задаём эффект перемещения
        }

        private void GameCanvas_Drop(object sender, DragEventArgs e)
        {
            // Логика размещения карты после перетаскивания
            var position = e.GetPosition(GameCanvas);
            if (_draggedElement != null)
            {
                Canvas.SetLeft(_draggedElement, position.X - (_draggedElement.RenderSize.Width / 2));
                Canvas.SetTop(_draggedElement, position.Y - (_draggedElement.RenderSize.Height / 2));
                _draggedElement = null;
            }

        }

        private void SaveScoreButton_Click(object sender, RoutedEventArgs e)
        {
            var playerNameWindow = new PlayerNameWindow();
            if (playerNameWindow.ShowDialog() == true)
            {
                string playerName = playerNameWindow.PlayerName;
                int score = _gameManager.GetScore();

                var recordsService = new RecordsService();
                recordsService.SaveRecord(playerName, score);

                MessageBox.Show("Результат сохранен!", "Сохранение");
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e) => _gameManager.StartNewGame();

        private void SaveGameButton_Click(object sender, RoutedEventArgs e) => _gameManager.SaveGame();

        private void LoadGameButton_Click(object sender, RoutedEventArgs e) => _gameManager.LoadGame();

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            _gameManager.RestartGame();
            RestartButton.IsEnabled = false; // После перезапуска отключаем кнопку
        }

        private void ViewRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            var recordsWindow = new RecordsWindow();
            recordsWindow.ShowDialog();
        }
    }
}
