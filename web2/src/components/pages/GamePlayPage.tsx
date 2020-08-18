import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Typography } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame } from '../../controllers/gameController';
import { GameStatus } from '../../api-client';
import { navigateTo } from '../../utilities/navigation';
import * as Routes from '../../utilities/routes';

const GamePlayPage: FC<GamePageProps> = ({ gameId }) => {
  const state = useSelector(selectActiveGame);

  useEffect(() => {
    if (state.game === null) {
      loadGame(gameId);
    } else if (![GameStatus.InProgress, GameStatus.Over].includes(state.game.status)) {
      navigateTo(Routes.game(gameId));
    }
  });

  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <Typography variant="h4">
        {`Game ${gameId} play page`}
      </Typography>
      <br />
      {state.game ? JSON.stringify(state.game) : ''}
    </div>
  );
};

export default GamePlayPage;
