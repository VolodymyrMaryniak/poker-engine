using System.Collections.Generic;

namespace Poker.Models
{
    public class Player
    {
        public string NickName { get; set; }
        public int ChipsOnStart { get; set; }
        public int Chips { get; set; }
        public int Position { get; set; }
        public SortedList<int, PlayerAction> Actions { get; set; }
    }

    public class PlayerWithCards
    {
        public string NickName { get; set; }
        public PocketCards PockerCards { get; set; }
    }
}
