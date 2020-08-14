import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const GameDiplomacyPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Game diplomacy page
    </div>
  );
};

export default GameDiplomacyPage;
