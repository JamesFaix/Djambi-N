using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services
{
    public class ValidationService
    {
        private static readonly Regex _validPlayerNameRegex = new Regex(@"^[A-Za-z0-9_\- ]{1,64}$");

        public static bool IsValidPlayerName(string name) =>
            _validPlayerNameRegex.IsMatch(name);
        
        public Result<Unit> ValidatePlayerNames(IEnumerable<string> playerNames)
        {
            if (playerNames == null)
            {
                return new ArgumentNullException(nameof(playerNames))
                    .ToErrorResult<Unit>();
            }

            var nameList = playerNames.ToList();

            if (nameList.Count < Constants.MinPlayerCount 
                || nameList.Count > Constants.MaxPlayerCount)
            {
                return new ArgumentException(
                    $"There must be between {Constants.MinPlayerCount} and {Constants.MaxPlayerCount} players.", 
                    nameof(playerNames))
                    .ToErrorResult<Unit>();
            }
            
            return AssertNamesAreValid(nameList)
                .Bind(_ => AssertNamesAreUnique(nameList));
        }

        public Result<Unit> ValidateNewPlayerName(IEnumerable<string> existingNames, string newName)
        {
            if (existingNames == null)
            {
                return new ArgumentNullException(nameof(existingNames))
                    .ToErrorResult<Unit>();
            }

            if (newName == null)
            {
                return new ArgumentNullException(nameof(newName))
                    .ToErrorResult<Unit>();
            }

            var nameList = existingNames.ToList();
            nameList.Add(newName);

            return AssertNamesAreValid(nameList)
                .Bind(_ => AssertNamesAreUnique(nameList));
        }

        private Result<Unit> AssertNamesAreValid(IEnumerable<string> names)
        {
            var invalidNames = names
               .Where(name => !IsValidPlayerName(name))
               .ToList();

            if (invalidNames.Any())
            {
                return new InvalidPlayerNamesException(
                    "Player names must be 1-64 characters in length and contain only letters, " + 
                    "numbers, - (dash), _ (underscore), and (space).",
                    invalidNames)
                   .ToErrorResult<Unit>();
            }
            else
            {
                return Unit.Value.ToResult();
            }
        }

        private Result<Unit> AssertNamesAreUnique(IEnumerable<string> names)
        {
            var namesGroupedBySimilarity = names
                .Select(name => new
                {
                    Input = name,
                    Simplified = GetSimplifiedPlayerName(name)
                })
                .GroupBy(name => name.Simplified);

            if (namesGroupedBySimilarity.Any(group => group.Count() > 1))
            {
                var invalidInputNames = namesGroupedBySimilarity
                    .Where(group => group.Count() > 1)
                    .SelectMany(group => group.Select(name => name.Input));

                return new InvalidPlayerNamesException(
                    $"Player names must be unique, ignoring capitalization and spaces.",
                    invalidInputNames)
                    .ToErrorResult<Unit>();
            }
            else
            {
                return Unit.Value.ToResult();
            }
        }

        internal string GetSimplifiedPlayerName(string name) =>
            name.Replace(" ", "").ToUpperInvariant();

        public Result<Unit> ValidateGameState(GameState game)
        {
            var errors = new List<Exception>();

            var piecesOutOfBounds = game.Pieces
                .Where(p => !p.Location.IsValid())
                .ToList();

            if (piecesOutOfBounds.Any())
            {
                errors.Add(new Exception($"The following pieces are in invalid locations.\n" +
                    string.Join("\n", piecesOutOfBounds.Select(p => $"{p.Type} {p.Location}"))));
            }

            var deadPlayerIds = game.Players
                .Where(p => !p.IsAlive)
                .Select(p => p.Id)
                .ToList();

            var piecesOwnedByDeadPlayer = game.Pieces
                .Where(p => p.PlayerId.HasValue 
                    && deadPlayerIds.Contains(p.PlayerId.Value))
                .ToList();

            if (piecesOwnedByDeadPlayer.Any())
            {
                errors.Add(new Exception($"The following pieces are controlled by a dead player.\n" +
                    string.Join("\n", piecesOwnedByDeadPlayer.Select(p => $"{p.Type} {p.Location}"))));
            }

            var piecesSharingLocation = game.Pieces
                .GroupBy(p => p.Location)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();

            if (piecesSharingLocation.Any())
            {
                errors.Add(new Exception($"The following pieces are stacked.\n" +
                    string.Join("\n", piecesSharingLocation.Select(p => $"{p.Type} {p.Location}"))));
            }

            return errors.Any()
                ? new AggregateException(errors).ToErrorResult<Unit>()
                : Unit.Value.ToResult();
        }

        public Result<Unit> ValidateTurnState(GameState game, TurnState turn)
        {
            var errors = new List<Exception>();

            if (turn.Status != TurnStatus.AwaitingConfirmation)
            {
                errors.Add(new Exception($"Turn status is {turn.Status}."));
            }

            if (turn.Selections.Count < 2 || turn.Selections.Count > 4)
            {
                errors.Add(new Exception("A turn must have between 2 and 4 selections."));
            }
            else
            {
                if (turn.Selections[0].Type != SelectionType.Subject)
                {
                    errors.Add(new Exception($"First selection must be {SelectionType.Subject}."));
                }

                if (!game.PiecesIndexedByLocation.TryGetValue(turn.Selections[0].Location, out var subject))
                {
                    errors.Add(new Exception($"First selection must be a location with a piece."));
                }

                if (subject.PlayerId == null || subject.PlayerId.Value != game.TurnCycle[0])
                {
                    errors.Add(new Exception("Subject must be controlled by the current player."));
                }

                switch (turn.Selections[1].Type)
                {
                    case SelectionType.MoveDestination:
                        if (game.PiecesIndexedByLocation.ContainsKey(turn.Selections[1].Location))
                        {
                            errors.Add(new Exception($"{SelectionType.MoveDestination} must have a location with no pieces."));
                        }
                        break;

                    case SelectionType.MoveDestinationWithTarget:
                        if (!game.PiecesIndexedByLocation.TryGetValue(turn.Selections[1].Location, out var target)
                            || target.PlayerId == subject.PlayerId)
                        {
                            errors.Add(new Exception($"{SelectionType.MoveDestinationWithTarget} must have a location with an enemy piece."));
                        }
                        break;

                    default:
                        errors.Add(new Exception($"Second selection must be {SelectionType.MoveDestination} or {SelectionType.MoveDestinationWithTarget}."));
                        break;
                }

                //TODO: Validate 3rd and 4th selections
            }

            return errors.Any()
                ? new AggregateException(errors).ToErrorResult<Unit>()
                : Unit.Value.ToResult();
        }
    }
}
