using System;

namespace Poker.Models
{
    public class PocketCards
    {
        public Card Card1 { get; }
        public Card Card2 { get; }

        public PocketCards(Card card1, Card card2)
        {
            var compareResult = card1.CompareTo(card2);
            
            if (compareResult == 0)
            {
                throw new ArgumentException();
            }

            if (compareResult < 0)
            {
                Card1 = card1;
                Card2 = card2;
            }
            else
            {
                Card1 = card2;
                Card2 = card1;
            }
        }
    }
}
