import React, { FC } from 'react';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import { Button, Drawer } from '@material-ui/core';
import { GameInfo, UserInfo } from '../../model/game';
import { PlayerKind, GameStatus } from '../../api-client';
import GamelessSection from './GamelessSection/GamelessSection';
import ActiveGameSection from './ActiveGameSection/ActiveGameSection';

const useStyles = makeStyles({
  list: {
    width: 250,
  },
  fullList: {
    width: 'auto',
  },
});

const NavigationDrawer: FC = () => {
  const classes = useStyles();
  const [isOpen, setIsOpen] = React.useState(true);

  const toggleDrawer = (open: boolean) => (event: any) => {
    if (event.type === 'keydown' && (event.key === 'Tab' || event.key === 'Shift')) {
      return;
    }

    setIsOpen(open);
  };

  let user: UserInfo | null = {
    id: 1,
    name: "Mr. User",
    privileges: []
  };

  let game: GameInfo | null = {
    id: 1,
    description: 'Ultrabattle!!!',
    createdBy: {
      userId: 1,
      userName: 'derp',
      time: Date.UTC(2020, 12, 31, 12, 59, 59) as unknown as Date
    },
    status: GameStatus.NUMBER_2,
    players: [
      { id: 1, name: 'derp', kind: PlayerKind.NUMBER_1, userId: 1 },
      { id: 2, name: 'flerp', kind: PlayerKind.NUMBER_2, userId: null }
    ]
  };

  return (
    <div>
      <Button onClick={toggleDrawer(true)}>
        Open navigation drawer
      </Button>
      <Drawer
        open={isOpen}
        onClose={toggleDrawer(false)}
      >
        <div
          className={clsx(classes.list, {
            [classes.fullList]: false,
          })}
          role="presentation"
          onClick={toggleDrawer(false)}
          onKeyDown={toggleDrawer(false)}
        >
          <GamelessSection user={user} />
          <ActiveGameSection game={game} />
        </div >
      </Drawer>
    </div>
  );
};

export default NavigationDrawer;