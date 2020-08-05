import React, { FC } from 'react';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import { Button, Divider, Drawer, List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import {
  AccountBalance as DiplomacyIcon,
  Add as NewGameIcon,
  CameraAlt as SnapshotsIcon,
  EmojiEvents as ResultsIcon,
  ExitToApp as SignOutIcon,
  Gavel as RulesIcon,
  Home as HomeIcon,
  Input as SignInIcon,
  MeetingRoom as LobbyIcon,
  PersonAdd as CreateAccountIcon,
  PlayArrow as PlayIcon,
  Search as SearchIcon,
  Settings as SettingsIcon
} from '@material-ui/icons';
import { GameInfo } from '../../model/game';
import { navigateTo } from '../../utilities/navigation';
import { PlayerKind } from '../../api-client';

interface NavigationItemProps {
  text: string;
  icon: any;
  onClick: () => void;
}

const NavigationItem: FC<NavigationItemProps> = ({ text, icon, onClick }) => {
  return (
    <ListItem button key={text} onClick={onClick} >
      <ListItemIcon>{icon}</ListItemIcon>
      <ListItemText primary={text} />
    </ListItem>
  );
};

const UnauthenticatedNavigationItems: FC = () => {
  return (
    <List>
      <NavigationItem text="Sign in" icon={<SignInIcon />} onClick={() => navigateTo('/sign-in')} />
      <NavigationItem text="Create account" icon={<CreateAccountIcon />} onClick={() => navigateTo('/create-account')} />
    </List>
  );
};

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

const GameNavigationItems: FC<{ game: GameInfo }> = ({ game }) => {
  return (
    <List>
      <NavigationItem text="Game lobby" icon={<LobbyIcon />} onClick={() => navigateTo(`/games/${game.id}/lobby`)} />
      <NavigationItem text="Resume game" icon={<PlayIcon />} onClick={() => navigateTo(`/games/${game.id}/play`)} />
      <NavigationItem text="Game diplomacy" icon={<DiplomacyIcon />} onClick={() => navigateTo(`/games/${game.id}/diplomacy`)} />
      <NavigationItem text="Game snapshots" icon={<SnapshotsIcon />} onClick={() => navigateTo(`/games/${game.id}/snapshots`)} />
      <NavigationItem text="Game outcome" icon={<ResultsIcon />} onClick={() => navigateTo(`/games/${game.id}/outcome`)} />
    </List>
  );
};

const useStyles = makeStyles({
  list: {
    width: 250,
  },
  fullList: {
    width: 'auto',
  },
});

export default function TemporaryDrawer() {
  const classes = useStyles();
  const [isOpen, setIsOpen] = React.useState(false);

  const toggleDrawer = (open: boolean) => (event: any) => {
    if (event.type === 'keydown' && (event.key === 'Tab' || event.key === 'Shift')) {
      return;
    }

    setIsOpen(open);
  };

  const game: GameInfo = {
    id: 1,
    description: 'some game',
    createdBy: {
      userId: 1,
      userName: 'derp',
      time: Date.UTC(2020, 12, 31, 12, 59, 59) as unknown as Date
    },
    players: [
      { id: 1, name: 'derp', kind: PlayerKind.NUMBER_1, userId: 1 },
      { id: 2, name: 'flerp', kind: PlayerKind.NUMBER_2, userId: null }
    ]
  };

  const list = () => (
    <div
      className={clsx(classes.list, {
        [classes.fullList]: false,
      })}
      role="presentation"
      onClick={toggleDrawer(false)}
      onKeyDown={toggleDrawer(false)}
    >
      <UnauthenticatedNavigationItems />
      <Divider />
      <GamelessNavigationItems />
      <Divider />
      <GameNavigationItems game={game} />
    </div >
  );

  return (
    <div>
      <Button onClick={toggleDrawer(true)}>
        Open navigation drawer
      </Button>
      <Drawer
        open={isOpen}
        onClose={toggleDrawer(false)}
      >
        {list()}
      </Drawer>
    </div>
  );
}
