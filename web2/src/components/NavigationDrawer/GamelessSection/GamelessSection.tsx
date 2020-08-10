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
} from '@material-ui/icons';
import { useSelector } from 'react-redux';
import { navigateTo } from '../../../utilities/navigation';
import NavigationItem from '../NavigationItem';
import { selectSession } from '../../../hooks/selectors';

const getUnauthenticatedItems = () => (
  <>
    <NavigationItem
      text="Sign in"
      icon={<SignInIcon />}
      onClick={() => navigateTo('/sign-in')}
    />
    <NavigationItem
      text="Create account"
      icon={<CreateAccountIcon />}
      onClick={() => navigateTo('/create-account')}
    />
  </>
);

const getAuthenticatedItems = () => (
  <>
    <NavigationItem
      text="Home"
      icon={<HomeIcon />}
      onClick={() => navigateTo('/home')}
    />
    <NavigationItem
      text="New game"
      icon={<NewGameIcon />}
      onClick={() => navigateTo('/new-game')}
    />
    <NavigationItem
      text="Search games"
      icon={<SearchIcon />}
      onClick={() => navigateTo('/search-games')}
    />
    <NavigationItem
      text="Sign out"
      icon={<SignOutIcon />}
      onClick={() => navigateTo('/sign-out')}
    />
  </>
);

const getConstantItems = () => (
  <>
    <NavigationItem
      text="Settings"
      icon={<SettingsIcon />}
      onClick={() => navigateTo('/settings')}
    />
    <NavigationItem
      text="Rules"
      icon={<RulesIcon />}
      onClick={() => navigateTo('/rules')}
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
