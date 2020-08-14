import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame } from '../../controllers/gameController';
import { GameStatus } from '../../api-client';
import PendingLobbyPlayersTable from '../tables/PendingLobbyPlayersTable';
import InProgressLobbyPlayersTable from '../tables/InProgressLobbyPlayersTable';

const GameLobbyPage: FC<GamePageProps> = ({ gameId }) => {
  const { game } = useSelector(selectActiveGame);

  useEffect(() => {
    if (game === null) {
      loadGame(gameId);
    }
  });

  if (game === null) {
    return <></>;
  }

  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} lobby page`}
      <br />
      {
        game.status === GameStatus.Pending
          ? <PendingLobbyPlayersTable />
          : <InProgressLobbyPlayersTable />
      }
    </div>
  );
};

export default GameLobbyPage;
