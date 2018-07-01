namespace Poker.Models
{
    public class PlayerPossibleActions
    {
        public string PlayerNickName { get; set; }
        public int? CallSize { get; set; }
        public int? MinBetSize { get; set; }
        public int PlayerChips { get; set; }
    }
}