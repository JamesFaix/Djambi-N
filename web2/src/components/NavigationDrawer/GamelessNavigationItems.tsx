import React, { FC } from 'react';
import { List } from '@material-ui/core';
import {
  Add as NewGameIcon,
  ExitToApp as SignOutIcon,
  Gavel as RulesIcon,
  Home as HomeIcon,
  Search as SearchIcon,
  Settings as SettingsIcon
} from '@material-ui/icons';
import { navigateTo } from '../../utilities/navigation';
import NavigationItem from './NavigationItem';

const GamelessNavigationItems: FC = () => {
  return (
    <List>
      <NavigationItem text="Home" icon={<HomeIcon />} onClick={() => navigateTo('/home')} />
      <NavigationItem text="Settings" icon={<SettingsIcon />} onClick={() => navigateTo('/settings')} />
      <NavigationItem text="New game" icon={<NewGameIcon />} onClick={() => navigateTo('/new-game')} />
      <NavigationItem text="Search games" icon={<SearchIcon />} onClick={() => navigateTo('/search-games')} />
      <NavigationItem text="Rules" icon={<RulesIcon />} onClick={() => navigateTo('/rules')} />
      <NavigationItem text="Sign out" icon={<SignOutIcon />} onClick={() => navigateTo('/sign-out')} />
    </List>
  );
};

export default GamelessNavigationItems;
