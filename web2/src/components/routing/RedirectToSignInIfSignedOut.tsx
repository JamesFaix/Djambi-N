import React, { FC } from 'react';
import { useSelector } from 'react-redux';
import { selectSession } from '../../hooks/selectors';
import { navigateTo } from '../../controllers/navigationController';
import * as Routes from '../../utilities/routes';

const RedirectToSignInIfSignedOut: FC = () => {
  const { user, isRestorePending } = useSelector(selectSession);
  const isSignedOut = !isRestorePending && user === null;

  if (isSignedOut) {
    navigateTo(Routes.signIn);
  }

  return <></>;
};

export default RedirectToSignInIfSignedOut;
