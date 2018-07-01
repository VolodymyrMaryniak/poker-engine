using Poker.Models;
using Poker.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Operations.Extentions
{
    public static class PlayerExtentions
    {
        public static bool Folded(this Player player)
        {
            return player.Actions.Any(x => x.ActionType == ActionType.Fold);
        }

        public static bool AllIn(this Player player)
        {
            return player.Actions.Any(x => x.ActionType == ActionType.AllIn);
        }

        public static int SpentChips(this Player player)
        {
            return player.Actions.Sum(x => x.Value.BetSize.GetValueOrDefault());
        }

        public static int SpentChipsOnStage(this Player player, GameStage stage)
        {
            return player.Actions.Where(x => x.GameStage == stage).Sum(x => x.BetSize.GetValueOrDefault());
        }

        public static bool ParticipatedOnTheStage(this Player player, GameStage stage)
        {
            if (player.Actions.Any(x => x.GameStage == stage))
            {
                return true;
            }

            return !player.Folded();
        }

        public static IEnumerable<PlayerAction> GetStageActions(this Player player, GameStage stage)
        {
            return player.Actions.Where(x => x.GameStage == stage);
        }
    }
}
