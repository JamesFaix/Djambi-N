import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Typography, Container, Button } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame, startGame } from '../../controllers/gameController';
import { GameStatus } from '../../api-client';
import LobbyPlayersTable from '../tables/LobbyPlayersTable/LobbyPlayersTable';
import GameParametersTable from '../tables/GameParametersTable';
import { theme } from '../../styles/materialTheme';
import { useFormStyles } from '../../styles/styles';
import { navigateTo } from '../../utilities/navigation';
import * as Routes from '../../utilities/routes';

const GameLobbyPage: FC<GamePageProps> = ({ gameId }) => {
  const { game } = useSelector(selectActiveGame);

  useEffect(() => {
    if (game === null) {
      loadGame(gameId);
    }
  });

  const styles = useFormStyles(theme);

  if (game === null) {
    return <></>;
  }

  if (game.status !== GameStatus.Pending) {
    navigateTo(Routes.gameInfo(game.id));
    return <></>;
  }

  const canStart = game.players.length >= 2;

  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <Typography variant="h4">
        {`Game ${gameId} lobby page`}
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
        <LobbyPlayersTable />
      </Container>
      <br />
      <br />
      <Container maxWidth="xs">
        <Button
          className={styles.button}
          onClick={() => startGame(game.id)}
          disabled={!canStart}
        >
          Start
        </Button>
        <br />
        <br />
        {canStart ? <></>
          : (
            <Typography variant="caption">
              Cannot start until more players to join.
            </Typography>
          )}
      </Container>
    </div>
  );
};

export default GameLobbyPage;
