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
import { UserInfo } from '../../../model/game';
import { navigateTo } from '../../../utilities/navigation';
import NavigationItem from '../NavigationItem';

const GamelessSection: FC<{ user: UserInfo | null }> = ({ user }) => {
  return (
    <List>
      {user ? (
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
      ) : (
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
        )}
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
    </List>
  );
};

export default GamelessSection;
