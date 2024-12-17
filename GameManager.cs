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
        private int _score;
        public event Action<int> ScoreUpdated; // Событие для обновления очков

        public GameManager(Canvas canvas)
        {
            _canvas = canvas;
            _gameState = new GameState();
            _score = 0; // Инициализируем очки
        }
        public void AddPoints(int points)
        {
            _score += points;
            UpdateScoreDisplay();
        }

        public int GetScore()
        {
            return _score;
        }
        // Метод вызывается, когда карта перевернута
        public void OnCardFlipped()
        {
            AddPoints(10); // Добавляем 10 очков за открытую карту
        }

        private void UpdateScoreDisplay()
        {
            // Отправляем событие в MainWindow для обновления UI
            ScoreUpdated?.Invoke(_score);
        }
        // Метод вызывается, когда Foundation завершен
        public void OnFoundationCompleted()
        {
            AddPoints(100); // Добавляем 100 очков за заполнение Foundation
        }
        private bool IsCompleteSequence(Deck pile)
        {
            var cards = pile.GetAllCards().ToList(); // Преобразуем IEnumerable<Card> в List<Card>

            if (cards.Count != 13)
            {
                return false; // Стопка не может быть последовательностью, если в ней меньше или больше карт
            }

            // Проверяем, что карты идут по порядку от короля до туза
            for (int i = 0; i < 13; i++)
            {
                var expectedRank = (Card.Rank)(Card.Rank.Ace + i); // От короля к тузу
                if (cards[i].CardRank != expectedRank) // Теперь можно использовать индекс
                {
                    return false;
                }
            }

            return true;
        }



        private void HandleCardDrop(Card draggedCard, Card targetCard)
        {
            // Проверка, что целевая карта (targetCard) является верхней картой в своей колонке
            if (!IsTopCard(targetCard))
            {
                MessageBox.Show("Невозможно переместить карту на карту, которая не является верхней!");
                return; // Отменяем перетаскивание
            }

            // Проверка, что перетаскиваемая карта является верхней картой
            if (!IsTopCard(draggedCard))
            {
                // Перетаскиваем все карты от выбранной до верхней
                var allCardsToMove = GetCardsToMove(draggedCard);
                MoveMultipleCards(allCardsToMove, targetCard);
            }
            else
            {
                // Проверка правил:
                // 1. Перемещение возможно только на карту противоположного цвета и на 1 ранг больше.
                if (IsMoveValid(draggedCard, targetCard))
                {
                    // Логика перемещения карты
                    MoveCard(draggedCard, targetCard);

                    // Обновляем графику
                    DrawGame();
                }
                else
                {
                    MessageBox.Show("Неверный ход!");
                }
            }
        }
        private bool IsTopCard(Card card)
        {
            if (GetPileContainingCard(card) != null)
            {
                // Проверяем, является ли карта верхней в своей колонке
                var pile = GetPileContainingCard(card);
                return pile.GetAllCards().First() == card;
            }
            return true;
        }

        private Deck GetPileContainingCard(Card card)
        {
            foreach (var pile in _gameState.TableauPiles)
            {
                if (pile.GetAllCards().Contains(card))
                {
                    return pile;
                }
            }

            return null; // Возвращаем null, если колода не найдена
        }


        private List<Card> GetCardsToMove(Card draggedCard)
        {
            // Получаем все карты от выбранной до верхней карты в колонке
            var pile = GetPileContainingCard(draggedCard);
            var allCards = pile.GetAllCards().ToList();
            allCards.Reverse();
            var startIndex = allCards.IndexOf(draggedCard);

            // Возвращаем список карт от выбранной до первой карты
            return allCards.GetRange(startIndex, allCards.Count - startIndex);
        }

        private void MoveMultipleCards(List<Card> cardsToMove, Card targetCard)
        {
            // Находим исходную стопку (из которой будем перемещать карты)
            var sourcePile = GetPileContainingCard(cardsToMove.Last());

            if (sourcePile == null || targetCard == null)
                return;

            // Проверка, что перемещение возможно
            if (IsMoveValid(cardsToMove.First(), targetCard))
            {
                // Удаляем карты из исходной стопки
                foreach (var card in cardsToMove)
                {
                    sourcePile.RemoveCard(card);
                }

                // Если после удаления карты в стопке не пустые, открываем верхнюю карту
                if (sourcePile.GetAllCards().Any())
                {
                    var topCard = sourcePile.GetAllCards().First();
                    if (!topCard.IsFaceUp)
                    {
                        OnCardFlipped();
                    }
                    topCard.IsFaceUp = true;
                }

                // Добавляем карты в целевую стопку
                Deck targetPile = GetPileContainingCard(targetCard);
                if (targetPile != null)
                {
                    foreach (var card in cardsToMove)
                    {
                        targetPile.AddCard(card);
                    }
                }

                // Проверяем, не образована ли последовательность из 13 карт
                if (IsCompleteSequence(targetPile))
                {
                    OnFoundationCompleted(); // Добавляем 100 очков

                    // Удаляем карты из поля
                    RemoveCardsFromField(targetPile);
                }

                // Обновляем графику
                DrawGame();
            }
            else
            {
                MessageBox.Show("Неверный ход!");
            }
        }

        private void RemoveCardsFromField(Deck completedPile)
        {
            // Удаляем карты, если последовательность завершена
            foreach (var card in completedPile.GetAllCards())
            {
                // Удаляем карту из поля
                _gameState.TableauPiles.Remove(completedPile); // Удаляем колоду с картами
            }
        }

        private bool IsMoveValid(Card draggedCard, Card targetCard = null)
        {
            if (targetCard == null) // Если целевая колонка пустая
            {
                return draggedCard.CardRank == Card.Rank.King; // Только король может быть перемещён
            }

            // Проверяем цвет
            bool isOppositeColor = (draggedCard.CardSuit == Card.Suit.Hearts || draggedCard.CardSuit == Card.Suit.Diamonds) ==
                                   (targetCard.CardSuit == Card.Suit.Clubs || targetCard.CardSuit == Card.Suit.Spades);

            // Проверяем ранг (должен быть на 1 меньше)
            bool isOneRankLower = (int)draggedCard.CardRank == (int)targetCard.CardRank - 1;

            return isOppositeColor && isOneRankLower;
        }

        private void MoveCard(Card draggedCard, Card targetCard)
        {
            Deck sourcePile = null;
            Deck targetPile = null;

            // Найти исходную стопку
            foreach (var pile in _gameState.TableauPiles)
            {
                if (pile.GetAllCards().Contains(draggedCard))
                {
                    sourcePile = pile;
                    break;
                }
            }

            if (_gameState.Waste.GetAllCards().Contains(draggedCard))
            {
                sourcePile = _gameState.Waste;
                OnCardFlipped();
            }

            // Найти целевую стопку
            foreach (var pile in _gameState.TableauPiles)
            {
                if (pile.GetAllCards().Contains(targetCard))
                {
                    targetPile = pile;
                    break;
                }
            }

            if (sourcePile != null && targetPile != null)
            {
                // Удаляем карту из исходной стопки
                sourcePile.RemoveCard(draggedCard);

                // Если убрали верхнюю карту из Tableau, открываем следующую карту
                if (sourcePile != _gameState.Waste && sourcePile.GetAllCards().Any())
                {
                    var topCard = sourcePile.GetAllCards().First();
                    if (topCard.IsFaceUp != true) OnCardFlipped();
                    topCard.IsFaceUp = true;
                    
                }

                // Добавляем карту в целевую стопку
                targetPile.AddCard(draggedCard);
            }
        }


        private void InitializeDragAndDrop(UIElement element, Card card)
        {
            if (!card.IsFaceUp)
            {
                // Закрытые карты нельзя перетаскивать
                return;
            }

            element.PreviewMouseLeftButtonDown += (sender, e) =>
            {
                DragDrop.DoDragDrop((DependencyObject)sender, card, DragDropEffects.Move);
            };

            element.Drop += (sender, e) =>
            {
                if (e.Data.GetData(typeof(Card)) is Card draggedCard)
                {
                    if (card == null) // Если целевая колонка пуста
                    {
                        if (IsMoveValid(draggedCard))
                        {
                            MoveCardToEmptyColumn(draggedCard);
                            DrawGame();
                        }
                        else
                        {
                            MessageBox.Show("Только короли могут перемещаться в пустую колонку!");
                        }
                    }
                    else
                    {
                        HandleCardDrop(draggedCard, card);
                    }
                }
            };


        }

        public void StartNewGame()
        {
            // Очистка текущего состояния
            _canvas.Children.Clear();
            _gameState.TableauPiles.Clear();
            _gameState.Waste = new Deck(new List<Card>()); // Очистка стека удержания
            _score = 0;
            UpdateScoreDisplay();

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
                var topWasteCard = _gameState.Waste.GetAllCards().First(); // Берём последнюю карту
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
                    HorizontalAlignment = HorizontalAlignment.Left, // Текст выравнивается слева
                    VerticalAlignment = VerticalAlignment.Top, // Текст выравнивается сверху
                    Margin = new Thickness(5), // Отступ от границы
                    FontWeight = FontWeights.Bold,
                    Foreground = (card.CardSuit == Card.Suit.Hearts || card.CardSuit == Card.Suit.Diamonds) ? Brushes.Red : Brushes.Black
                };

                border.Child = text;
            }

            // Добавляем Drag-and-Drop обработчики
            InitializeDragAndDrop(border, card);

            return border;
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
                _gameState.Waste.AddCard(card); // Добавляем карту в Waste
                DrawGame();
            }
        }
        private void MoveCardToEmptyColumn(Card draggedCard)
        {
            Deck sourcePile = null;

            // Найти исходную стопку
            foreach (var pile in _gameState.TableauPiles)
            {
                if (pile.GetAllCards().Contains(draggedCard))
                {
                    sourcePile = pile;
                    break;
                }
            }

            if (_gameState.Waste.GetAllCards().Contains(draggedCard))
            {
                sourcePile = _gameState.Waste;
            }

            if (sourcePile != null)
            {
                sourcePile.RemoveCard(draggedCard);

                // Создаём новую стопку в пустой колонке
                var emptyPile = new Deck(new[] { draggedCard });
                _gameState.TableauPiles.Add(emptyPile);
            }
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

                Score = _score // Сохраняем текущие очки
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

            // Восстановление очков
            _score = saveData.Score;
            UpdateScoreDisplay(); // Обновляем отображение очков

            // Перерисовка игрового поля
            DrawGame();
        }


        public void RestartGame()
        {
            // Перемещаем все карты из Waste обратно в Stock
            foreach (var card in _gameState.Waste.GetAllCards())
            {
                _gameState.Stock.AddCard(card);  // Добавляем каждую карту в Stock
            }

            // Очищаем Waste
            _gameState.Waste = new Deck(new List<Card>());

            // Отнимаем 25 очков за перезапуск
            AddPoints(-25);

            // Перерисовываем игровое поле
            DrawGame();
        }


    }
    public class SaveData
    {
        public List<CardData> Stock { get; set; }
        public List<CardData> Waste { get; set; }
        public List<List<CardData>> Tableau { get; set; }
        public int Score { get; set; } // Новое поле для очков
    }

    public class CardData
    {
        public int CardSuit { get; set; }
        public int CardRank { get; set; }
        public bool IsFaceUp { get; set; }
    }


}
