import React, { FC } from 'react';
import { List } from '@material-ui/core';
import {
  Input as SignInIcon,
  PersonAdd as CreateAccountIcon,
  Add as NewGameIcon,
  ExitToApp as SignOutIcon,
  Home as HomeIcon,
  Search as SearchIcon,
  Gavel as RulesIcon,
  Settings as SettingsIcon,
  Notifications as NotificationsIcon,
} from '@material-ui/icons';
import { useSelector } from 'react-redux';
import NavigationItem from '../NavigationItem';
import { selectSession } from '../../../hooks/selectors';
import * as Routes from '../../../utilities/routes';

const getUnauthenticatedItems = () => (
  <>
    <NavigationItem
      text="Sign in"
      icon={<SignInIcon />}
      path={Routes.signIn}
    />
    <NavigationItem
      text="Create account"
      icon={<CreateAccountIcon />}
      path={Routes.createAccount}
    />
  </>
);

const getAuthenticatedItems = () => (
  <>
    <NavigationItem
      text="Home"
      icon={<HomeIcon />}
      path={Routes.home}
    />
    <NavigationItem
      text="New game"
      icon={<NewGameIcon />}
      path={Routes.newGame}
    />
    <NavigationItem
      text="Search games"
      icon={<SearchIcon />}
      path={Routes.searchGames}
    />
    <NavigationItem
      text="Sign out"
      icon={<SignOutIcon />}
      path={Routes.signOut}
    />
  </>
);

const getConstantItems = () => (
  <>
    <NavigationItem
      text="Notifications"
      icon={<NotificationsIcon />}
      path={Routes.notifications}
    />
    <NavigationItem
      text="Settings"
      icon={<SettingsIcon />}
      path={Routes.settings}
    />
    <NavigationItem
      text="Rules"
      icon={<RulesIcon />}
      path={Routes.rules}
    />
  </>
);

const GamelessSection: FC = () => {
  const session = useSelector(selectSession);

  return (
    <List>
      {session.user ? getAuthenticatedItems() : getUnauthenticatedItems()}
      {getConstantItems()}
    </List>
  );
};

export default GamelessSection;
