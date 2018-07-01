using System;
using Poker.Models.Enums;

namespace Poker.Models
{
    public struct Card : IComparable
    {
        public Rank Rank { get; set; }
        public Suit Suit { get; set; }

        public int CompareTo(object obj)
        {
            if (!(obj is Card card)) throw new ArgumentException();

            var rankCompare = Rank.CompareTo(card.Rank);
            return rankCompare == 0 ? Suit.CompareTo(card.Suit) : rankCompare;
        }

        public static bool operator ==(Card card1, Card card2)
        {
            return card1.Rank == card2.Rank && card1.Suit == card2.Suit;
        }

        public static bool operator !=(Card card1, Card card2)
        {
            return !(card1 == card2);
        }

        public static bool operator <(Card card1, Card card2)
        {
            return card1.CompareTo(card2) < 0;
        }

        public static bool operator >(Card card1, Card card2)
        {
            return card1.CompareTo(card2) > 0;
        }
    }
}
