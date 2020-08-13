import React, { FC } from 'react';
import { useSelector } from 'react-redux';
import { selectSession } from '../../hooks/selectors';
import { navigateTo } from '../../utilities/navigation';
import * as Routes from '../../utilities/routes';

interface Props {
  requiresAuthentication: boolean
}

const RedirectBasedOnAuthenticationStatus: FC<Props> = ({ requiresAuthentication }) => {
  const session = useSelector(selectSession);

  if (requiresAuthentication && session.user === null) {
    navigateTo(Routes.signIn);
  } else if (!requiresAuthentication && session.user !== null) {
    navigateTo(Routes.home);
  }

  return <></>;
};

export default RedirectBasedOnAuthenticationStatus;
