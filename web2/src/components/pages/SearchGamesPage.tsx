import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const SearchGamesPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Search games page
    </div>
  );
};

export default SearchGamesPage;
