using System;

namespace Poker.Operations
{
    public static class ExceptionFactory
    {
        public static Exception GameIsAlreadyFinished => new Exception("THE_GAME_IS_ALREADY_FINISHED");
        public static Exception GameIsNotFinishedYet => new Exception("THE_GAME_IS_NOT_FINISHED_YET");
        public static Exception PlayerHasNotEnoughChips => new Exception("PLAYER_HAS_NOT_ENOUGH_CHIPS");
        public static ArgumentException NotUniqueNickName => new ArgumentException("ALL_PLAYERS_SHOULD_HAVE_UNIQUE_NICK_NAME");
        public static ArgumentException NotUniquePlayerPosition => new ArgumentException("ALL_PLAYERS_SHOULD_HAVE_UNIQUE_POSITION");
        public static ArgumentException NotAllPlayersHaveChips => new ArgumentException("ALL_PLAYERS_SHOULD_HAVE_CHIPS");
        public static ArgumentException UnknownButtonPosition => new ArgumentException("UNKNOWN_BUTTON_POSITION");
        public static Exception UnknownPlayer => new Exception("UNKNOWN_PLAYER");
    }
}
