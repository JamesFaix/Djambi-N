import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';

const GameDiplomacyPage: FC<GamePageProps> = ({ gameId }) => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} diplomacy page`}
    </div>
  );
};

export default GameDiplomacyPage;
