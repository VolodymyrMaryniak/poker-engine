using Poker.Models;
using Poker.Models.Enums;
using Poker.Operations.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Operations.Game
{
    public partial class Game
    {
        private readonly SortedList<int, Player> _players;
        private readonly SortedList<int, PlayerAction> _allActions;

        private readonly int _buttonPosition;
        private readonly int _smallBlindSize;
        private readonly int _bigBlindSize;
        private readonly int _anteSize;

        private GameStage _currentGameStage;

        private IEnumerable<Player> CurrentStagePlayers => _players
            .Where(x => x.ParticipatedOnTheStage(_currentGameStage));

        private IEnumerable<PlayerAction> CurrentStageActions => _allActions
            .Where(x => x.GameStage == _currentGameStage);

        private bool IsStartOfNewStage => !_allActions.Any(x => x.GameStage == _currentGameStage);

        private bool IsEndOfGame => _players.Count(x => !x.Folded()) == 1;

        private bool DidAllFoldOrGoAllIn => _players.Count(x => !x.Folded() && !x.AllIn()) < 2;

        private bool DidAllFold => _players.Count(x => !x.Folded()) < 2;

        private int MaxBetOnCurrentStage => CurrentStageActions.Any()
            ? CurrentStageActions.Max(x => x.BetSize.GetValueOrDefault())
            : 0;

        private void PutAnteAndBlinds()
        {
            var playerOnButton = FindPlayerByPosition(_buttonPosition);
            var playerOnSmallBlind = FindNextPlayer(playerOnButton);
            var playerOnBigBlind = FindNextPlayer(playerOnSmallBlind);

            foreach (var player in _players.ToValueList())
            {
                AddAction(player, new PlayerAction
                {
                    ActionType = ActionType.Ante,
                    BetSize = _anteSize
                });
            }

            AddAction(playerOnSmallBlind, new PlayerAction
            {
                ActionType = ActionType.SmallBlind,
                BetSize = _smallBlindSize
            });

            AddAction(playerOnBigBlind, new PlayerAction
            {
                ActionType = ActionType.BigBlind,
                BetSize = _bigBlindSize
            });
        }

        private PlayerPossibleActions GetPossibleActions()
        {
            var player = GetCurrentPlayer();
            var alreadyBet = player.SpentChipsOnStage(_currentGameStage);
            var maxBet = MaxBetOnCurrentStage;
            var minBetSize = GetMinBetSize();

            return new PlayerPossibleActions
            {
                PlayerNickName = player.NickName,
                PlayerChips = player.Chips,
                CallSize = maxBet != alreadyBet ? maxBet - alreadyBet : (int?)null,
                MinBetSize = maxBet + minBetSize - alreadyBet
            };
        }

        private int GetMinBetSize()
        {
            var minBetSize = _bigBlindSize;
            var actions = _allActions
                .Where(x => x.ActionType == ActionType.Raise ||
                            x.ActionType == ActionType.AllIn ||
                            x.ActionType == ActionType.BigBlind)
                .ToList();

            for (var i = 0; i < actions.Count - 1; i++)
            {
                var difference = actions[i + 1].BetSize.Value - actions[i].BetSize.Value;
                if (difference > minBetSize)
                {
                    minBetSize = difference;
                }
            }

            return minBetSize;
        }

        private bool IsEndOfTheStage()
        {
            var allSaidSomething = CurrentStagePlayers
                .All(player => player.GetStageActions(_currentGameStage).Any());
            if (allSaidSomething)
            {
                var maxBetSize = MaxBetOnCurrentStage;
                if (maxBetSize == 0)
                {
                    return true;
                }

                return CurrentStagePlayers.All(x => x.Folded() || x.AllIn() ||
                    x.SpentChipsOnStage(_currentGameStage) == maxBetSize);
            }

            return false;
        }

        private Player GetCurrentPlayer()
        {
            if (IsStartOfNewStage)
            {
                var buttonPlayer = FindPlayerByPosition(_buttonPosition);
                return FindNextPlayer(buttonPlayer);
            }

            return FindNextPlayer(_allActions.Last().Value.Player);
        }

        private Player FindPlayer(string nickName)
        {
            var player = _players.FirstOrDefaultValue(x => x.NickName == nickName);
            ValidatePlayer(player);

            return player;
        }

        private Player FindPlayerByPosition(int position)
        {
            var player = _players.FirstOrDefaultValue(x => x.Position == position);
            ValidatePlayer(player);

            return player;
        }

        private Player FindNextPlayer(Player player)
        {
            var nextPlayer = _players
                .FirstOrDefault(x => x.Key > player.Position &&
                                    !x.Value.AllIn() && !x.Value.Folded())
                .Value;

            nextPlayer = nextPlayer ?? _players.FirstOrDefaultValue(x => !x.AllIn() && !x.Folded());

            ValidatePlayer(nextPlayer);
            return nextPlayer;
        }

        private void AddAction(Player player, PlayerAction action)
        {
            var actionNumber = _allActions.LastOrDefault().Key + 1;
            action.ActionNumber = actionNumber;
            action.Player = player;
            action.GameStage = _currentGameStage;

            if (player.Chips < action.BetSize && 
                action.GameStage == GameStage.Preflop && (
                    action.ActionType == ActionType.Ante ||
                    action.ActionType == ActionType.SmallBlind ||
                    action.ActionType == ActionType.BigBlind))
            {
                action.ActionType = ActionType.AllIn;
                action.BetSize = player.Chips;
            }

            switch (action.ActionType)
            {
                case ActionType.Call:
                case ActionType.Raise:
                case ActionType.AllIn:
                case ActionType.SmallBlind:
                case ActionType.BigBlind:
                case ActionType.Ante:
                    Bet(player, action.BetSize.Value);
                    break;
            }

            player.Actions.Add(action.ActionNumber, action);
            _allActions.Add(action.ActionNumber, action);
        }

        private void ValidateEndOfTheStage()
        {
            if (IsEndOfGame)
            {
                IsGameFinished = true;
                return;
            }

            if (IsEndOfTheStage())
            {
                if (_currentGameStage == GameStage.River)
                {
                    IsGameFinished = true;
                    return;
                }

                if (DidAllFoldOrGoAllIn)
                {
                    EveryoneFoldOrGoAllIn = true;
                    IsGameFinished = true;
                    EveryoneFold = DidAllFold;
                }

                _currentGameStage += 1;
            }
        }

        private static void Bet(Player player, int betSize)
        {
            if (player.Chips < betSize)
            {
                throw ExceptionFactory.PlayerHasNotEnoughChips;
            }

            player.Chips -= betSize;
        }

        private static void ValidateInputPlayersCollection(List<Player> players, int buttonPosition)
        {
            if (players.Select(x => x.NickName).Distinct().Count() < players.Count)
            {
                throw ExceptionFactory.NotUniqueNickName;
            }

            if (players.Select(x => x.Position).Distinct().Count() < players.Count)
            {
                throw ExceptionFactory.NotUniquePlayerPosition;
            }

            if (players.Any(x => x.Chips <= 0))
            {
                throw ExceptionFactory.NotAllPlayersHaveChips;
            }

            if (players.All(x => x.Position != buttonPosition))
            {
                throw ExceptionFactory.UnknownButtonPosition;
            }
        }

        private static void ValidatePlayer(Player player)
        {
            if (player == null)
            {
                throw ExceptionFactory.UnknownPlayer;
            }
        }

        private GameResult CalculateResult()
        {
            var playersInGame = _players.Where(x => !x.Folded()).ToList();
            var bank = _allActions.Sum(x => x.Value.BetSize.GetValueOrDefault());

            if (playersInGame.Count == 1)
            {
                playersInGame.First().Chips += bank;
                return new GameResult
                {
                    Players = _players.ToValueList()
                };
            }

            throw new Exception();
        }
        private GameResult CalculateResult(List<PlayerWithCards> playersWithCards, TableCards tableCards)
        {
            var playersInGame = _players.Where(x => !x.Folded()).ToList();
            var bank = _allActions.Sum(x => x.Value.BetSize.GetValueOrDefault());

            if (playersInGame.Count == 1)
            {
                throw new Exception();
            }

            var players = playersInGame
                .Select(x => new
                {
                    x.NickName,
                    SpendChips = x.SpentChips(),
                    playersWithCards.First(y => y.NickName == x.NickName).PockerCards
                })
                .Select(x => new
                {
                    x.NickName,
                    x.SpendChips,
                    MaxBank = _players.Sum(y => y.ChipsOnStart - y.Chips > x.SpendChips ? x.SpendChips : y.ChipsOnStart - y.Chips),
                    CombinationStrength = StrengthOperations.GetStrength(new List<Card>
                    {
                        tableCards.FlopCard1,
                        tableCards.FlopCard2,
                        tableCards.FlopCard3,
                        tableCards.TurnCard,
                        tableCards.RiverCard,
                        x.PockerCards.Card1,
                        x.PockerCards.Card2
                    })
                })
                .OrderByDescending(x => x.CombinationStrength)
                .ToList();


            // If we don't have equals strength of combinations.
            //
            //var alreadyPaid = 0;
            //foreach (var p in players)
            //{
            //    if (p.MaxBank <= alreadyPaid)
            //    {
            //        continue;
            //    }

            //    var player = FindPlayer(p.NickName);
            //    player.Chips += p.MaxBank - alreadyPaid;
            //    alreadyPaid = p.MaxBank;

            //    if (alreadyPaid == bank)
            //    {
            //        break;
            //    }
            //}


            // We need divide chips if have equals strength of combinations.
            //
            // 28 24
            // 2  2  2 1  3
            // 18 20 5 10 30 // 83
            // 12 16 0 45 10

            var alreadyPaid = 0;
            foreach (var group in players.GroupBy(x => x.CombinationStrength).OrderByDescending(x => x.Key).ToList())
            { 
                var orderedGroup = group.ToList();
                while (orderedGroup.Any())
                {
                    var minMaxBank = orderedGroup.Min(x => x.MaxBank);
                    if (minMaxBank <= alreadyPaid)
                    {
                        orderedGroup.RemoveAll(x => x.MaxBank <= minMaxBank);
                        continue;
                    }

                    var toPay = (minMaxBank - alreadyPaid) / orderedGroup.Count; // accuracy may be lost here

                    orderedGroup.ForEach(x =>
                    {
                        var player = FindPlayer(x.NickName);
                        player.Chips += toPay;
                    });

                    orderedGroup.RemoveAll(x => x.MaxBank <= minMaxBank);

                    alreadyPaid = minMaxBank;
                }

                if (alreadyPaid == bank)
                {
                    break;
                }
            }

            return new GameResult
            {
                Players = _players.ToValueList()
            };
        }
    }
}
