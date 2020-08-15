import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const HomePage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Home page
    </div>
  );
};

export default HomePage;
