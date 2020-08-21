import React, { FC } from 'react';
import { useSelector } from 'react-redux';
import { selectSession } from '../../hooks/selectors';
import { navigateTo } from '../../controllers/navigationController';
import * as Routes from '../../utilities/routes';

const RedirectToHomeIfSignedIn: FC = () => {
  const session = useSelector(selectSession);

  if (session.user !== null) {
    navigateTo(Routes.home);
  }

  return <></>;
};

export default RedirectToHomeIfSignedIn;
