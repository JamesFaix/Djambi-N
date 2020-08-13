import React, { FC } from 'react';
import SignInForm from '../forms/SignInForm';
import RedirectToHomeIfSignedIn from '../routing/RedirectToHomeIfSignedIn';

const SignInPage: FC = () => {
  return (
    <div>
      <RedirectToHomeIfSignedIn />
      <SignInForm />
    </div>
  );
};

export default SignInPage;
