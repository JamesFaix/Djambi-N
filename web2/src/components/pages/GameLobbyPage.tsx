import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';

const GameLobbyPage: FC<GamePageProps> = ({ gameId }) => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} lobby page`}
    </div>
  );
};

export default GameLobbyPage;
