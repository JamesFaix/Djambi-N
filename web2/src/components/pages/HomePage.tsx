import React, { FC } from 'react';
import { Typography } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const HomePage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <Typography variant="h4">
        Home
      </Typography>
      <br />
    </div>
  );
};

export default HomePage;
