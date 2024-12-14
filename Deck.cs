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
    }
}
