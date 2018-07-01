using Poker.Models.Enums;

namespace Poker.Models
{
    public class PlayerAction
    {
        public int ActionNumber { get; set; }
        public Player Player { get; set; }
        public ActionType ActionType { get; set; }
        public int? BetSize { get; set; }
        public GameStage GameStage { get; set; }
    }
}
