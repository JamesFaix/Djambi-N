import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import { Typography, Container, Button } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame, startGame } from '../../controllers/gameController';
import { GameStatus } from '../../api-client';
import PendingGamePlayersTable from '../tables/PendingGamePlayersTable/PendingGamePlayersTable';
import InProgressLobbyPlayersTable from '../tables/InProgressLobbyPlayersTable';
import GameParametersTable from '../tables/GameParametersTable';
import { theme } from '../../styles/materialTheme';
import { useFormStyles } from '../../styles/styles';

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

  const isPending = game.status === GameStatus.Pending;
  const canStart = isPending && game.players.length >= 2;

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
        {
          isPending
            ? <PendingGamePlayersTable />
            : <InProgressLobbyPlayersTable />
        }
      </Container>
      <br />
      <br />
      {isPending
        ? (
          <>
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
          </>
        )
        : <></>}
    </div>
  );
};

export default GameLobbyPage;
