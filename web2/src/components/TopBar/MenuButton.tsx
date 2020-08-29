import React, { FC } from 'react';
import { IconButton } from '@material-ui/core';
import { Menu as MenuIcon } from '@material-ui/icons';
import { useSelector } from 'react-redux';
import { selectNavigation } from '../../hooks/selectors';
import { toggleDrawer } from '../../controllers/navigationController';
import { topBarStyles } from './styles';

const MenuButton: FC = () => {
  const classes = topBarStyles();
  const state = useSelector(selectNavigation);
  const isOpen = state.isDrawerOpen;

  const toggle = () => {
    toggleDrawer(!isOpen);
  };

  return (
    <IconButton
      edge="start"
      className={classes.button}
      onClick={toggle}
    >
      <MenuIcon />
    </IconButton>
  );
};

export default MenuButton;
