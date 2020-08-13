import React, { FC } from 'react';
import { useSelector } from 'react-redux';
import { selectSession } from '../../hooks/selectors';
import { navigateTo } from '../../utilities/navigation';
import * as Routes from '../../utilities/routes';

const RedirectToSignInIfSignedOut: FC = () => {
  const session = useSelector(selectSession);

  if (session.user === null) {
    navigateTo(Routes.signIn);
  }

  return <></>;
};

export default RedirectToSignInIfSignedOut;
