import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Typography } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame } from '../../controllers/gameController';
import { GameStatus } from '../../api-client';
import { navigateTo } from '../../controllers/navigationController';
import * as Routes from '../../utilities/routes';

const GamePlayPage: FC<GamePageProps> = ({ gameId }) => {
  const { game } = useSelector(selectActiveGame);

  useEffect(() => {
    if (game === null) {
      loadGame(gameId);
    } else if (![GameStatus.InProgress, GameStatus.Over].includes(game.status)) {
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
      <Typography variant="body1">
        {game ? JSON.stringify(game) : ''}
      </Typography>
    </div>
  );
};

export default GamePlayPage;
