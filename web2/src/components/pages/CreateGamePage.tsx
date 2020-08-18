import React, { FC } from 'react';
import { Typography } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import CreateGameForm from '../forms/CreateGameForm';

const CreateGamePage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <Typography variant="h4">
        Create game
      </Typography>
      <br />
      <CreateGameForm />
    </div>
  );
};

export default CreateGamePage;
