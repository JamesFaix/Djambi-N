import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const GameSnapshotsPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Game snapshots page
    </div>
  );
};

export default GameSnapshotsPage;
