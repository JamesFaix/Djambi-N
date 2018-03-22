using System;
using System.Collections.Generic;
using System.Text;
using Djambi.Engine.Extensions;
using Djambi.Model;

namespace Djambi.Engine.Services
{
    class GameUpdateService
    {
        private readonly ValidationService _validationService = new ValidationService();

        public Result<GameState> UpdateGameState(GameState game, TurnState turn)
        {
            return _validationService.ValidateTurnState(game, turn)
                .Bind(_ =>
                {
                    /*
                     * Change subject location to s[1] location
                     * 
                     * If s[1] is MoveDestinationWithTarget
                     *     If subject in [Chief, Militant, Assassin], kill target
                     *     If subject is Assassin, move target to s[0] location
                     *     If subject in [Chief, Militant, Diplomat, Necromobile], drop target at s[2] location
                     * 
                     * If subject is reporter and s[2] is Target, kill target
                     * 
                     * If subject in [Assassin, Diplomat, Necromobile] and s[1] location is the Maze, move subject to s[n] location
                     * 
                     * If target is Chief, set owning player's IsAlive = false and all other owned pieces PlayerId to subject's owner's Id
                     * 
                     * If subject or target is Chief and s[1] location is the Maze, update the turn cycle (expand description)
                     * 
                     * Check for surrounded Chiefs (expand description)
                     * 
                     * Check for stalemate (expand description)
                     */

                    return _validationService.ValidateGameState(game)
                        .Map(__ => game);
                });           
        }
    }
}
