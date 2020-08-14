import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import CreateGameForm from '../forms/CreateGameForm';

const CreateGamePage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <CreateGameForm />
    </div>
  );
};

export default CreateGamePage;
