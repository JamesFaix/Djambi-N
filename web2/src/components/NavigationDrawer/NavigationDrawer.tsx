import React, { FC } from 'react';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import { Button, Drawer } from '@material-ui/core';
import { GameInfo } from '../../model/game';
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

  const toggleDrawer = (open: boolean) => (event: React.MouseEvent | React.KeyboardEvent) => {
    if (event.type === 'keydown') {
      const e = event as React.KeyboardEvent;
      if (e.key === 'Tab' || e.key === 'Shift') {
        return;
      }
    }

    setIsOpen(open);
  };

  return (
    <div>
      <Button onClick={toggleDrawer(true)}>Open navigation drawer</Button>
      <Drawer open={isOpen} onClose={toggleDrawer(false)}>
        <div
          className={clsx(classes.list, {
            [classes.fullList]: false,
          })}
          role="presentation"
          onClick={toggleDrawer(false)}
          onKeyDown={toggleDrawer(false)}
        >
          <GamelessSection />
          <ActiveGameSection />
        </div>
      </Drawer>
    </div>
  );
};

export default NavigationDrawer;
