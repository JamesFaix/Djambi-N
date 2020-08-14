import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';

const GameSnapshotsPage: FC<GamePageProps> = ({ gameId }) => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} snapshots page`}
    </div>
  );
};

export default GameSnapshotsPage;
