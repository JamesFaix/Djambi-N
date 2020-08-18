import React, { FC } from 'react';
import { Typography } from '@material-ui/core';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const SearchGamesPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      <Typography variant="h4">
        Search games
      </Typography>
    </div>
  );
};

export default SearchGamesPage;
