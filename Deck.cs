using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire.Models
{
    public class Deck
    {
        private readonly Stack<Card> _cards = new Stack<Card>();

        public Deck(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
                _cards.Push(card);
        }

        public Card DrawCard() => _cards.Pop();

        public bool IsEmpty() => _cards.Count == 0;

        public IEnumerable<Card> GetAllCards() => _cards;

        public void AddCard(Card card)
        {
            _cards.Push(card);
        }

        public bool RemoveCard(Card card)
        {
            // Преобразуем Stack в список для удобной работы
            var cardList = _cards.ToList();

            if (cardList.Remove(card)) // Если карта найдена и удалена
            {
                // Перезаполняем Stack после удаления
                _cards.Clear();
                foreach (var remainingCard in cardList.AsEnumerable().Reverse()) // Возвращаем карты в изначальном порядке
                    _cards.Push(remainingCard);

                return true; // Успешно удалено
            }

            return false; // Карта не найдена
        }
    }
}
