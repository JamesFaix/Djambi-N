import React from 'react';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import { Button, Divider, Drawer } from '@material-ui/core';
import { GameInfo } from '../../model/game';
import { PlayerKind } from '../../api-client';
import UnauthenticatedNavigationItems from './UnauthenticatedNavigationItems';
import GamelessNavigationItems from './GamelessNavigationItems';
import ActiveGameNavigationItems from './ActiveGameNavigationItems';

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
      <ActiveGameNavigationItems game={game} />
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
