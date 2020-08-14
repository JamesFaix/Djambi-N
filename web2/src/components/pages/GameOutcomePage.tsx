import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const GameOutcomePage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Game outcome page
    </div>
  );
};

export default GameOutcomePage;
