import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const GamePage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Game page
    </div>
  );
};

export default GamePage;
