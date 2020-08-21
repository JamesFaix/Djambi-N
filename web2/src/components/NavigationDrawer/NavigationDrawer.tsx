import React, { FC } from 'react';
import clsx from 'clsx';
import { makeStyles } from '@material-ui/core/styles';
import { Drawer } from '@material-ui/core';
import { useSelector } from 'react-redux';
import GamelessSection from './GamelessSection/GamelessSection';
import ActiveGameSection from './ActiveGameSection/ActiveGameSection';
import { selectNavigation } from '../../hooks/selectors';
import { toggleDrawer } from '../../controllers/navigationController';

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
  const state = useSelector(selectNavigation);
  const isOpen = state.isDrawerOpen;

  const close = (event: React.MouseEvent | React.KeyboardEvent) => {
    if (event.type === 'keydown') {
      const e = event as React.KeyboardEvent;
      if (e.key === 'Tab' || e.key === 'Shift') {
        return;
      }
    }

    toggleDrawer(false);
  };

  return (
    <div>
      <Drawer open={isOpen} onClose={close}>
        <div
          className={clsx(classes.list, {
            [classes.fullList]: false,
          })}
          role="presentation"
          onClick={close}
          onKeyDown={close}
        >
          <GamelessSection />
          <ActiveGameSection />
        </div>
      </Drawer>
    </div>
  );
};

export default NavigationDrawer;
