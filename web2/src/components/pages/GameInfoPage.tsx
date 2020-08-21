import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Typography, Container } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame } from '../../controllers/gameController';
import { GameStatus } from '../../api-client';
import InProgressLobbyPlayersTable from '../tables/InProgressLobbyPlayersTable';
import GameParametersTable from '../tables/GameParametersTable';
import { navigateTo } from '../../utilities/navigation';
import * as Routes from '../../utilities/routes';

const GameInfoPage: FC<GamePageProps> = ({ gameId }) => {
  const { game } = useSelector(selectActiveGame);

  useEffect(() => {
    if (game === null) {
      loadGame(gameId);
    }
  });

  if (game === null) {
    return <></>;
  }

  if (game.status === GameStatus.Pending) {
    navigateTo(Routes.gameLobby(game.id));
    return <></>;
  }

  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <Typography variant="h4">
        {`Game ${gameId} info page`}
      </Typography>
      <br />
      <Container maxWidth="xs">
        <Typography variant="h5">
          Settings
        </Typography>
        <GameParametersTable />
      </Container>
      <br />
      <br />
      <Container maxWidth="sm">
        <Typography variant="h5">
          Players
        </Typography>
        <InProgressLobbyPlayersTable />
      </Container>
    </div>
  );
};

export default GameInfoPage;
