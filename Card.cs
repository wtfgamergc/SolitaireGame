using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire.Models
{
    public class Card
    {
        public enum Suit { Hearts, Diamonds, Clubs, Spades }
        public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

        public Suit CardSuit { get; }
        public Rank CardRank { get; }
        public bool IsFaceUp { get; set; }

        public Card(Suit suit, Rank rank)
        {
            CardSuit = suit;
            CardRank = rank;
            IsFaceUp = false;
        }
    }
}
