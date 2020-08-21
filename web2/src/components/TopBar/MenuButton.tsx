import React, { FC } from 'react';
import { IconButton, makeStyles } from '@material-ui/core';
import { Menu as MenuIcon } from '@material-ui/icons';
import { useSelector } from 'react-redux';
import { selectNavigation } from '../../hooks/selectors';
import { toggleDrawer } from '../../controllers/navigationController';

const useStyles = makeStyles((theme) => ({
  menuButton: {
    marginRight: theme.spacing(2),
    color: theme.palette.text.secondary,
  },
}));

const MenuButton: FC = () => {
  const classes = useStyles();

  const state = useSelector(selectNavigation);
  const isOpen = state.isDrawerOpen;

  const toggle = () => {
    toggleDrawer(!isOpen);
  };

  return (
    <IconButton
      edge="start"
      className={classes.menuButton}
      onClick={toggle}
    >
      <MenuIcon />
    </IconButton>
  );
};

export default MenuButton;
