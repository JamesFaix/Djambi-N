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

interface NavigationItemProps {
  text: string;
  icon: any;
}

const NavigationItem: FC<NavigationItemProps> = ({ text, icon }) => {
  return (
    <ListItem button key={text}>
      <ListItemIcon>{icon}</ListItemIcon>
      <ListItemText primary={text} />
    </ListItem>
  );
};

const UnauthenticatedNavigationItems: FC = () => {
  return (
    <List>
      <NavigationItem text="Sign in" icon={<SignInIcon />} />
      <NavigationItem text="Create account" icon={<CreateAccountIcon />} />
    </List>
  );
};

const GamelessNavigationItems: FC = () => {
  return (
    <List>
      <NavigationItem text="Home" icon={<HomeIcon />} />
      <NavigationItem text="Settings" icon={<SettingsIcon />} />
      <NavigationItem text="New game" icon={<NewGameIcon />} />
      <NavigationItem text="Search games" icon={<SearchIcon />} />
      <NavigationItem text="Rules" icon={<RulesIcon />} />
      <NavigationItem text="Sign out" icon={<SignOutIcon />} />
    </List>
  );
};

const GameNavigationItems: FC = () => {
  return (
    <List>
      <NavigationItem text="Game lobby" icon={<LobbyIcon />} />
      <NavigationItem text="Resume game" icon={<PlayIcon />} />
      <NavigationItem text="Game diplomacy" icon={<DiplomacyIcon />} />
      <NavigationItem text="Game snapshots" icon={<SnapshotsIcon />} />
      <NavigationItem text="Game outcome" icon={<ResultsIcon />} />
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
      <GameNavigationItems />
    </div>
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
