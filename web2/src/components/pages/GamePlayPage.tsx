import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';

const GamePlayPage: FC<GamePageProps> = ({ gameId }) => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} play page`}
    </div>
  );
};

export default GamePlayPage;
