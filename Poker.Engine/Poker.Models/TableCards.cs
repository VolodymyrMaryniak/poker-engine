using System;
using System.Collections.Generic;

namespace Poker.Models
{
    public class TableCards
    {
        public TableCards() { }

        public TableCards(IReadOnlyList<Card> cards)
        {
            if (cards.Count != 5)
            {
                throw new ArgumentException();
            }

            FlopCard1 = cards[0];
            FlopCard2 = cards[1];
            FlopCard3 = cards[2];
            TurnCard = cards[3];
            RiverCard = cards[4];
        }

        public Card FlopCard1 { get; set; }
        public Card FlopCard2 { get; set; }
        public Card FlopCard3 { get; set; }
        public Card TurnCard { get; set; }
        public Card RiverCard { get; set; }
    }
}
