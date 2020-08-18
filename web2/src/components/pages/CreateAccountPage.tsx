import React, { FC } from 'react';
import { Typography } from '@material-ui/core';
import RedirectToHomeIfSignedIn from '../routing/RedirectToHomeIfSignedIn';
import CreateAccountForm from '../forms/CreateAccountForm';

const CreateAccountPage: FC = () => {
  return (
    <div>
      <RedirectToHomeIfSignedIn />
      <Typography variant="h4">
        Create account
      </Typography>
      <br />
      <CreateAccountForm />
    </div>
  );
};

export default CreateAccountPage;
