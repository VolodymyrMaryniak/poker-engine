using Poker.Models;
using Poker.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Operations
{
    public static class StrengthOperations
    {
        public const int CountOfRanks = 13;

        public static int GetStrength(List<Card> cards)
        {
            switch (cards.Count)
            {
                case 5:
                    return GetStrengthFromFiveCards(cards);
                case 7:
                    return GetStrengthFromSevenCards(cards, out _);
            }

            throw new ArgumentException();
        }

        private static int GetStrengthFromFiveCards(IReadOnlyCollection<Card> cards)
        {
            var suits = cards.Select(x => x.Suit).ToList();
            var ranks = cards.Select(x => x.Rank).ToList();

            var isStraight = IsStraight(ranks, out var straightFrom);
            var isFlush = IsFlush(suits);

            if (isStraight && isFlush)
            {
                // Straight Flush from 'straightFrom'
                return GetStraightFlushStrength(straightFrom);
            }

            if (isStraight)
            {
                // Straight from 'straightFrom'
                return GetStraightStrength(straightFrom);
            }

            if (isFlush)
            {
                // Flush
                return GetFlushStrength(ranks);
            }

            if (HasFourOfAKind(ranks))
            {
                // Four Of A Kind
                return GetFourOfAKindStrength(ranks);
            }

            if (HasTwoPairs(ranks))
            {
                // Two Pairs
                return GetTwoPairStrength(ranks);
            }

            var hasThreeOfAKind = HasThreeOfAKind(ranks);
            var hasPair = HasPair(ranks);

            if (hasThreeOfAKind && hasPair)
            {
                // Full House
                return GetFullHouseStrength(ranks);
            }

            if (hasThreeOfAKind)
            {
                // Three Of A Kind
                return GetThreeOfAKindStrength(ranks);
            }

            if (hasPair)
            {
                // Pair
                return GetPairStrength(ranks);
            }

            // High Card
            return GetHighCardStrength(ranks);
        }

        private static int GetStrengthFromSevenCards(IEnumerable<Card> cards, out List<Card> bestFiveCards)
        {
            //var suits = cards.Select(x => x.Suit).ToList();
            //var ranks = cards.Select(x => x.Rank).ToList();

            //var isStraight = IsStraight(ranks, out var straightFrom);
            //var isFlush = IsFlush(suits);


            bestFiveCards = null;
            var strengthOfBest = int.MinValue;

            var k = 0;
            var cardsDictionary = cards.ToDictionary(x => k++);

            for (var i = 0; i < 6; i++)
            {
                for (var j = i + 1; j < 7; j++)
                {
                    var fiveCards = cardsDictionary.Where(x => x.Key != i && x.Key != j).Select(x => x.Value).ToList();
                    var strength = GetStrengthFromFiveCards(fiveCards);

                    if (strength > strengthOfBest)
                    {
                        strengthOfBest = strength;
                        bestFiveCards = fiveCards;
                    }
                }
            }

            return strengthOfBest;
        }

        private static bool HasPair(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();

            if (distinctRanks.Count == cards.Count)
            {
                return false;
            }

            return distinctRanks.Any(distRank => cards.Count(rank => rank == distRank) == 2);
        }

        private static bool HasTwoPairs(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();

            if (distinctRanks.Count > cards.Count - 2)
            {
                return false;
            }

            return distinctRanks.Count(distRank => cards.Count(rank => rank == distRank) == 2) == 2;
        }

        private static bool HasThreeOfAKind(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();

            if (distinctRanks.Count > cards.Count - 2)
            {
                return false;
            }

            return distinctRanks.Any(distRank => cards.Count(rank => rank == distRank) == 3);
        }

        private static bool HasFourOfAKind(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();
            if (distinctRanks.Count > cards.Count - 3)
            {
                return false;
            }

            return distinctRanks.Any(distRank => cards.Count(rank => rank == distRank) == 4);
        }

        private static bool IsStraight(ICollection<Rank> cards, out Rank straightFrom)
        {
            straightFrom = Rank.RankA;

            var orderedCards = cards.Distinct().OrderBy(x => x).ToList();
            if (orderedCards.Count < 5)
            {
                return false;
            }

            for (var i = orderedCards.Count - 3; i > 0; i--)
            {
                straightFrom = orderedCards[i];
                if (straightFrom > Rank.Rank10)
                {
                    return false;
                }

                if (orderedCards.Contains(straightFrom + 1) &&
                    orderedCards.Contains(straightFrom + 2) &&
                    orderedCards.Contains(straightFrom + 3) &&
                    orderedCards.Contains(straightFrom + 4))
                {
                    return true;
                }
            }

            if (cards.Contains(Rank.RankA) &&
                cards.Contains(Rank.Rank2) &&
                cards.Contains(Rank.Rank3) &&
                cards.Contains(Rank.Rank4) &&
                cards.Contains(Rank.Rank5))
            {
                straightFrom = Rank.RankA;
                return true;
            }

            return false;
        }

        private static bool IsFlush(IReadOnlyCollection<Suit> cards)
        {
            var distinctSuits = cards.Distinct().ToList();
            return distinctSuits.Max(distSuit => cards.Count(suit => suit == distSuit)) >= 5;
        }

        private static int GetHighCardStrength(IEnumerable<Rank> cards)
        {
            var orderedRanks = cards.Select(x => (int)x).OrderByDescending(x => x).ToList();

            var strength = 0;

            strength |= orderedRanks[0] << 24;
            strength |= orderedRanks[1] << 20;
            strength |= orderedRanks[2] << 16;
            strength |= orderedRanks[3] << 12;
            strength |= orderedRanks[4] << 8;

            return -strength;
        }

        private static int GetPairStrength(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();
            var pairRank = distinctRanks.First(x => cards.Count(y => y == x) == 2);

            var orderedRanks = cards
                .Except(new List<Rank> { pairRank, pairRank })
                .Select(x => (int)x)
                .OrderByDescending(x => x)
                .ToList();

            var strength = 0;

            strength |= (int)pairRank << 20;
            strength |= orderedRanks[0] << 16;
            strength |= orderedRanks[1] << 12;
            strength |= orderedRanks[2] << 8;

            return -strength;
        }

        private static int GetTwoPairStrength(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();
            var pairsRanks = distinctRanks.Where(x => cards.Count(y => y == x) == 2).OrderByDescending(x => x).ToList();

            var lastCard = cards.First(x => x != pairsRanks[0] && x != pairsRanks[1]);

            var strength = 0;

            strength |= (int)pairsRanks[0] << 16;
            strength |= (int)pairsRanks[1] << 12;
            strength |= (int)lastCard << 8;

            return -strength;
        }

        private static int GetThreeOfAKindStrength(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();
            var setRank = distinctRanks.First(x => cards.Count(y => y == x) == 3);

            var orderedCards = cards.Where(x => x != setRank).OrderByDescending(x => x).ToList();

            var strength = 0;

            strength |= (int)setRank << 12;
            strength |= (int)orderedCards[0] << 8;
            strength |= (int)orderedCards[1] << 4;

            return -strength;
        }

        private static int GetStraightStrength(Rank straightFrom)
        {
            var strength = 0;
            var value = straightFrom == Rank.RankA ? 1 : (int)straightFrom + 1;

            strength |= value << 8;

            return -strength;
        }

        private static int GetFlushStrength(IEnumerable<Rank> cards)
        {
            var orderedRanks = cards.Select(x => (int)x).OrderByDescending(x => x).ToList();

            var strength = 0;

            strength |= orderedRanks[0] << 16;
            strength |= orderedRanks[1] << 12;
            strength |= orderedRanks[2] << 8;
            strength |= orderedRanks[3] << 4;
            strength |= orderedRanks[4];

            return strength;
        }

        private static int GetFullHouseStrength(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();
            var setRank = distinctRanks.First(x => cards.Count(y => y == x) == 3);
            var pairRank = distinctRanks.First(x => x != setRank);

            var strength = 0;

            strength |= (int)setRank << 20;
            strength |= (int)pairRank << 16;

            return strength;
        }

        private static int GetFourOfAKindStrength(IReadOnlyCollection<Rank> cards)
        {
            var distinctRanks = cards.Distinct().ToList();
            var fourOfAKindRank = distinctRanks.First(x => cards.Count(y => y == x) == 4);
            var highCard = distinctRanks.First(x => x != fourOfAKindRank);

            var strength = 0;

            strength |= (int)fourOfAKindRank << 24;
            strength |= (int)highCard << 20;

            return strength;
        }

        private static int GetStraightFlushStrength(Rank straightFrom)
        {
            var strength = 0;
            var value = straightFrom == Rank.RankA ? 1 : (int)straightFrom + 1;

            strength |= value << 28;

            return strength;
        }
    }
}
