import React, { FC } from 'react';
import { List } from '@material-ui/core';
import {
  Input as SignInIcon,
  PersonAdd as CreateAccountIcon,
} from '@material-ui/icons';
import { navigateTo } from '../../utilities/navigation';
import NavigationItem from './NavigationItem';

const UnauthenticatedNavigationItems: FC = () => {
  return (
    <List>
      <NavigationItem text="Sign in" icon={<SignInIcon />} onClick={() => navigateTo('/sign-in')} />
      <NavigationItem text="Create account" icon={<CreateAccountIcon />} onClick={() => navigateTo('/create-account')} />
    </List>
  );
};

export default UnauthenticatedNavigationItems;
