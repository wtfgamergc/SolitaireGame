using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire.Models
{
    public class GameState
    {
        public List<Deck> TableauPiles { get; set; }
        public Deck Stock { get; set; }
        public Deck Waste { get; set; }
        public List<Deck> FoundationPiles { get; set; }

        public GameState()
        {
            TableauPiles = new List<Deck>();
            Stock = new Deck(new List<Card>());
            Waste = new Deck(new List<Card>());
            FoundationPiles = new List<Deck>();
        }
        

    }
}
