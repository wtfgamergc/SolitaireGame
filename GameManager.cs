using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Solitaire.Models;

namespace Solitaire.Services
{
    public class GameManager
    {
        private readonly Canvas _canvas;
        private readonly GameState _gameState;
        private UIElement _draggedElement;
        private Point _dragStartPoint;

        public GameManager(Canvas canvas)
        {
            _canvas = canvas;
            _gameState = new GameState();
        }
        public void InitializeDragAndDrop(UIElement cardControl)
        {
            // Добавляем события для карты
            cardControl.MouseMove += Card_MouseMove;
            cardControl.MouseLeftButtonDown += Card_MouseLeftButtonDown;
            cardControl.MouseLeftButtonUp += Card_MouseLeftButtonUp;
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _draggedElement = sender as UIElement;
            _dragStartPoint = e.GetPosition(_canvas);
            if (_draggedElement != null)
            {
                Panel.SetZIndex(_draggedElement, 100); // Переместить элемент на передний план
            }
        }

        private void Card_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(_canvas);
                double offsetX = currentPoint.X - _dragStartPoint.X;
                double offsetY = currentPoint.Y - _dragStartPoint.Y;

                // Перемещаем элемент
                Canvas.SetLeft(_draggedElement, Canvas.GetLeft(_draggedElement) + offsetX);
                Canvas.SetTop(_draggedElement, Canvas.GetTop(_draggedElement) + offsetY);

                _dragStartPoint = currentPoint;
            }
        }

        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedElement != null)
            {
                Panel.SetZIndex(_draggedElement, 0); // Вернуть элемент на исходный слой
                _draggedElement = null;
            }
        }
        public void StartNewGame()
        {
            // Очистка текущего состояния
            _canvas.Children.Clear();
            _gameState.TableauPiles.Clear();
            _gameState.FoundationPiles.Clear();
            _gameState.Waste = new Deck(new List<Card>()); // Очистка стека удержания

            // Создание новой колоды
            var fullDeck = CreateFullDeck().OrderBy(_ => Guid.NewGuid()).ToList();
            var deck = new Deck(fullDeck);

            // Раздача карт на Tableau
            for (int i = 0; i < 7; i++)
            {
                var tableauCards = new List<Card>();
                for (int j = 0; j <= i; j++)
                {
                    var card = deck.DrawCard();
                    card.IsFaceUp = j == i; // Открыть последнюю карту
                    tableauCards.Add(card);
                }

                var tableauDeck = new Deck(tableauCards);
                _gameState.TableauPiles.Add(tableauDeck);
            }

            // Остаток в стопку Stock
            _gameState.Stock = new Deck(deck.GetAllCards());

            // Отрисовка начального состояния
            DrawGame();
        }

        private void DrawGame()
        {
            _canvas.Children.Clear();

            // Отображение Tableau
            double startX = 50;
            double startY = 100;
            double cardOffsetY = 30;

            for (int columnIndex = 0; columnIndex < _gameState.TableauPiles.Count; columnIndex++)
            {
                var pile = _gameState.TableauPiles[columnIndex];
                double x = startX + columnIndex * 120;
                double y = startY + 70;

                foreach (var card in pile.GetAllCards().Reverse())
                {
                    var cardControl = CreateCardControl(card);
                    Canvas.SetLeft(cardControl, x);
                    Canvas.SetTop(cardControl, y);

                    _canvas.Children.Add(cardControl);

                    y += card.IsFaceUp ? cardOffsetY : 30; // Смещение меньше для закрытых карт
                }
            }

            // Отображение колоды Stock
            if (!_gameState.Stock.IsEmpty())
            {
                var stockTopCard = CreateCardControl(new Card(Card.Suit.Clubs, Card.Rank.Ace)); // Заглушка
                Canvas.SetLeft(stockTopCard, startX);
                Canvas.SetTop(stockTopCard, startY - 90); // Условная позиция
                _canvas.Children.Add(stockTopCard);
            }

            // Отображение Waste (стека удержания)
            if (!_gameState.Waste.IsEmpty())
            {
                var topWasteCard = _gameState.Waste.GetAllCards().First();
                var wasteControl = CreateCardControl(topWasteCard);
                Canvas.SetLeft(wasteControl, startX + 120);
                Canvas.SetTop(wasteControl, startY - 90); // Условная позиция
                _canvas.Children.Add(wasteControl);
            }
        }
        private UIElement CreateCardControl(Card card)
        {
            var border = new Border
            {
                Width = 100,
                Height = 140,
                CornerRadius = new CornerRadius(5),
                Background = card.IsFaceUp ? Brushes.White : Brushes.Gray,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            if (card.IsFaceUp)
            {
                var text = new TextBlock
                {
                    Text = $"{GetRankSymbol(card.CardRank)} {GetSuitSymbol(card.CardSuit)}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Foreground = (card.CardSuit == Card.Suit.Hearts || card.CardSuit == Card.Suit.Diamonds) ? Brushes.Red : Brushes.Black
                };
                border.Child = text;
            }

            // Добавляем Drag-and-Drop обработчики
            InitializeDragAndDrop(border);

            return border;
        }

        private void DrawCard(double x, double y, Card card, double width, double height)
        {
            var cardRect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = card.IsFaceUp ? Brushes.White : Brushes.Gray,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            var cardText = new TextBlock
            {
                Text = card.IsFaceUp ? $"{GetRankSymbol(card.CardRank)}{GetSuitSymbol(card.CardSuit)}" : "",
                Foreground = Brushes.Black,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(cardRect, x);
            Canvas.SetTop(cardRect, y);

            Canvas.SetLeft(cardText, x + 10);
            Canvas.SetTop(cardText, y + 10);

            _canvas.Children.Add(cardRect);
            _canvas.Children.Add(cardText);
        }


        private string GetRankSymbol(Card.Rank rank)
        {
            switch (rank)
            {
                case Card.Rank.Ace:
                    return "A";
                case Card.Rank.Jack:
                    return "J";
                case Card.Rank.Queen:
                    return "Q";
                case Card.Rank.King:
                    return "K";
                default:
                    return ((int)rank).ToString();
            }
        }

        private string GetSuitSymbol(Card.Suit suit)
        {
            switch (suit)
            {
                case Card.Suit.Hearts:
                    return "♥";
                case Card.Suit.Diamonds:
                    return "♦";
                case Card.Suit.Clubs:
                    return "♣";
                case Card.Suit.Spades:
                    return "♠";
                default:
                    return "?";
            }
        }
        public void MoveCardFromStockToWaste()
        {
            if (!_gameState.Stock.IsEmpty())
            {
                var card = _gameState.Stock.DrawCard();
                card.IsFaceUp = true; // Карта должна быть лицом вверх
                _gameState.Waste = new Deck(new[] { card });
                DrawGame();
            }
        }


        private void DrawCardBack(double x, double y, double width, double height)
        {
            var cardBack = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(cardBack, x);
            Canvas.SetTop(cardBack, y);

            _canvas.Children.Add(cardBack);
        }
        private void DrawEmptySlot(double x, double y, double width, double height)
        {
            var emptySlot = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };

            Canvas.SetLeft(emptySlot, x);
            Canvas.SetTop(emptySlot, y);

            _canvas.Children.Add(emptySlot);
        }

        private static IEnumerable<Card> CreateFullDeck()
        {
            foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
            {
                foreach (Card.Rank rank in Enum.GetValues(typeof(Card.Rank)))
                {
                    yield return new Card(suit, rank);
                }
            }
        }

        public void SaveGame()
        {
            var saveData = new SaveData
            {
                Stock = _gameState.Stock.GetAllCards().Select(card => new CardData
                {
                    CardSuit = (int)card.CardSuit,
                    CardRank = (int)card.CardRank,
                    IsFaceUp = card.IsFaceUp
                }).ToList(),

                Waste = _gameState.Waste.GetAllCards().Select(card => new CardData
                {
                    CardSuit = (int)card.CardSuit,
                    CardRank = (int)card.CardRank,
                    IsFaceUp = card.IsFaceUp
                }).ToList(),

                Tableau = _gameState.TableauPiles.Select(pile => pile.GetAllCards().Select(card => new CardData
                {
                    CardSuit = (int)card.CardSuit,
                    CardRank = (int)card.CardRank,
                    IsFaceUp = card.IsFaceUp
                }).ToList()).ToList(),

                Foundation = _gameState.FoundationPiles.Select(pile => pile.GetAllCards().Select(card => new CardData
                {
                    CardSuit = (int)card.CardSuit,
                    CardRank = (int)card.CardRank,
                    IsFaceUp = card.IsFaceUp
                }).ToList()).ToList()
            };

            var saveJson = System.Text.Json.JsonSerializer.Serialize(saveData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText("game_save.json", saveJson);
        }


        public void LoadGame()
        {
            // Путь к сохраненному файлу
            var saveFilePath = "game_save.json";
            if (!System.IO.File.Exists(saveFilePath)) return;

            // Чтение и десериализация JSON
            var saveJson = System.IO.File.ReadAllText(saveFilePath);
            var saveData = System.Text.Json.JsonSerializer.Deserialize<SaveData>(saveJson);

            if (saveData == null) return;

            // Восстановление Stock (в обратном порядке для корректного стека)
            _gameState.Stock = new Deck(saveData.Stock
                .ToList() // Убедимся, что это список
                .AsEnumerable() // Преобразуем в последовательность
                .Reverse() // Переворачиваем порядок
                .Select(card => new Card((Card.Suit)card.CardSuit, (Card.Rank)card.CardRank) { IsFaceUp = card.IsFaceUp }));

            // Восстановление Waste
            _gameState.Waste = new Deck(saveData.Waste
                .ToList() // Убедимся, что это список
                .AsEnumerable() // Преобразуем в последовательность
                .Reverse() // Переворачиваем порядок
                .Select(card => new Card((Card.Suit)card.CardSuit, (Card.Rank)card.CardRank) { IsFaceUp = card.IsFaceUp }));

            // Восстановление Tableau
            _gameState.TableauPiles = saveData.Tableau
                .Select(pile => new Deck(pile
                    .ToList() // Убедимся, что это список
                    .AsEnumerable() // Преобразуем в последовательность
                    .Reverse() // Переворачиваем порядок
                    .Select(card => new Card((Card.Suit)card.CardSuit, (Card.Rank)card.CardRank) { IsFaceUp = card.IsFaceUp })))
                .ToList();

            // Восстановление Foundation
            _gameState.FoundationPiles = saveData.Foundation
                .Select(pile => new Deck(pile
                    .ToList() // Убедимся, что это список
                    .AsEnumerable() // Преобразуем в последовательность
                    .Reverse() // Переворачиваем порядок
                    .Select(card => new Card((Card.Suit)card.CardSuit, (Card.Rank)card.CardRank) { IsFaceUp = card.IsFaceUp })))
                .ToList();

            // Перерисовка игрового поля
            DrawGame();
        }




        public void RestartGame()
        {
            // Логика перезапуска
            StartNewGame();
        }

    }
    public class SaveData
    {
        public List<CardData> Stock { get; set; }
        public List<CardData> Waste { get; set; }
        public List<List<CardData>> Tableau { get; set; }
        public List<List<CardData>> Foundation { get; set; }
    }

    public class CardData
    {
        public int CardSuit { get; set; }
        public int CardRank { get; set; }
        public bool IsFaceUp { get; set; }
    }


}
