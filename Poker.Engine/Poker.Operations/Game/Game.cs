using Poker.Models;
using Poker.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Operations.Game
{
    public partial class Game
    {
        public bool IsGameFinished { get; private set; }
        public bool EveryoneFoldOrGoAllIn { get; private set; }
        public bool EveryoneFold { get; private set; }

        public Game(List<Player> players, int buttonPosition, int smallBlindSize, int bigBlindSize, int anteSize)
        {
            ValidateInputPlayersCollection(players, buttonPosition);

            _allActions = new SortedList<int, PlayerAction>();
            _players = new SortedList<int, Player>();
            players.OrderBy(x => x.Position).ToList()
                .ForEach(x => _players.Add(x.Position, new Player
                {
                    NickName = x.NickName,
                    Chips = x.Chips,
                    ChipsOnStart = x.Chips,
                    Position = x.Position,
                    Actions = new SortedList<int, PlayerAction>()
                }));

            _anteSize = anteSize;
            _smallBlindSize = smallBlindSize;
            _bigBlindSize = bigBlindSize;
            _buttonPosition = buttonPosition;

            _currentGameStage = GameStage.Preflop;
            IsGameFinished = false;
            EveryoneFoldOrGoAllIn = false;
            EveryoneFold = false;

            PutAnteAndBlinds();
        }

        public PlayerPossibleActions GetPlayerPossibleActions()
        {
            if (IsGameFinished)
            {
                throw ExceptionFactory.GameIsAlreadyFinished;
            }

            return GetPossibleActions();
        }

        public void AddAction(string nickName, PlayerAction action)
        {
            if (IsGameFinished)
            {
                throw ExceptionFactory.GameIsAlreadyFinished;
            }

            var player = FindPlayer(nickName);
            AddAction(player, action);
            ValidateEndOfTheStage();
        }

        public GameResult GetResult()
        {
            if (!IsGameFinished)
            {
                throw ExceptionFactory.GameIsNotFinishedYet;
            }

            return CalculateResult();
        }
        public GameResult GetResult(List<PlayerWithCards> playersWithCards, TableCards tableCards)
        {
            if (!IsGameFinished)
            {
                throw ExceptionFactory.GameIsNotFinishedYet;
            }

            return CalculateResult(playersWithCards, tableCards);
        }
    }
}
