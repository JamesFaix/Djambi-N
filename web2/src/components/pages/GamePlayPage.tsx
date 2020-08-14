import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const GamePlayPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Game play page
    </div>
  );
};

export default GamePlayPage;
