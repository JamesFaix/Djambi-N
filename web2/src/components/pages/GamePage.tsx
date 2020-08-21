import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame } from '../../controllers/gameController';
import { GameStatus } from '../../api-client';
import { navigateTo } from '../../controllers/navigationController';
import * as Routes from '../../utilities/routes';

export interface GamePageProps {
  gameId: number
}

const GamePage: FC<GamePageProps> = ({ gameId }) => {
  const state = useSelector(selectActiveGame);

  useEffect(() => {
    if (state.game === null) {
      loadGame(gameId);
      return;
    }

    switch (state.game.status) {
      case GameStatus.InProgress:
        navigateTo(Routes.gamePlay(gameId));
        return;
      case GameStatus.Canceled:
      case GameStatus.Pending:
        navigateTo(Routes.gameLobby(gameId));
        return;
      case GameStatus.Over:
        navigateTo(Routes.gameOutcome(gameId));
        return;
      default:
        throw new Error(`Invalid game status ${state.game.status}`);
    }
  });

  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} page`}
      <br />
      {state.game ? JSON.stringify(state.game) : ''}
    </div>
  );
};

export default GamePage;
