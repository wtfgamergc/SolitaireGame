using Solitaire.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireGame
{
    public abstract class CardCollection
    {
        protected Stack<Card> _cards = new Stack<Card>();

        // Добавить карту в коллекцию
        public abstract void AddCard(Card card);

        // Удалить карту из коллекции
        public abstract bool RemoveCard(Card card);

        // Вытянуть карту из коллекции (например, для игры)
        public abstract Card DrawCard();

        // Проверить, пуста ли коллекция
        public abstract bool IsEmpty();

        // Получить все карты коллекции
        public abstract IEnumerable<Card> GetAllCards();
    }

}
