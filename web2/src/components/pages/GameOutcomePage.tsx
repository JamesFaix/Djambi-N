import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';

const GameOutcomePage: FC<GamePageProps> = ({ gameId }) => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} outcome page`}
    </div>
  );
};

export default GameOutcomePage;
