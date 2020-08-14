import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const GameLobbyPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Game lobby page
    </div>
  );
};

export default GameLobbyPage;
