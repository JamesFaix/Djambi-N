import React, { FC } from 'react';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';

const SignOutPage: FC = () => {
  return (
    <div>
      <RedirectToSignInIfSignedOut />
      Sign out page
    </div>
  );
};

export default SignOutPage;
