using SolitaireGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire.Models
{
    public class Deck : CardCollection
    {
        public Deck(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                _cards.Push(card);
            }
        }

        // Переопределяем метод добавления карты
        public override void AddCard(Card card)
        {
            _cards.Push(card);
        }

        // Переопределяем метод удаления карты
        public override bool RemoveCard(Card card)
        {
            var cardList = _cards.ToList();
            if (cardList.Remove(card))
            {
                // Перезаполняем Stack после удаления
                _cards.Clear();
                foreach (var remainingCard in cardList.AsEnumerable().Reverse())
                {
                    _cards.Push(remainingCard);
                }
                return true;
            }
            return false;
        }

        // Переопределяем метод вытягивания карты
        public override Card DrawCard() => _cards.Pop();

        // Переопределяем метод проверки на пустоту
        public override bool IsEmpty()
        {
            return _cards.Count == 0;
        }


        // Переопределяем метод получения всех карт
        public override IEnumerable<Card> GetAllCards() => _cards;
    }

}
