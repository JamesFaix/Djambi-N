using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Djambi.Model;

namespace Djambi.Engine
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

        public Result<Unit> ValidateGameState(GameState state)
        {
            var piecesOutOfBounds = state.Pieces
                .Where(p => !p.Location.IsValid())
                .ToList();

            if (piecesOutOfBounds.Any())
            {
                return new Exception($"The following pieces are in invalid locations.\n" +
                    string.Join("\n", piecesOutOfBounds.Select(p => $"{p.Type} {p.Location}")))
                    .ToErrorResult<Unit>();
            }

            return Unit.Value.ToResult();
        }
    }
}
