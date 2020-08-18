import React, { FC } from 'react';
import { Typography } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const SignOutPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <Typography variant="h4">
        Sign out
      </Typography>
    </div>
  );
};

export default SignOutPage;
