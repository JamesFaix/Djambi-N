import React, { FC } from 'react';
import RedirectToHomeIfSignedIn from '../routing/RedirectToHomeIfSignedIn';
import CreateAccountForm from '../forms/CreateAccountForm';

const CreateAccountPage: FC = () => {
  return (
    <div>
      <RedirectToHomeIfSignedIn />
      <CreateAccountForm />
    </div>
  );
};

export default CreateAccountPage;
