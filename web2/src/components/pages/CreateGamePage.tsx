import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const CreateGamePage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Create game page
    </div>
  );
};

export default CreateGamePage;
