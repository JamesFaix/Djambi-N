import React, { FC } from 'react';
import {
  AppBar, Toolbar, IconButton, Typography, makeStyles,
} from '@material-ui/core';
import { Menu as MenuIcon } from '@material-ui/icons';
import logo from '../../assets/logo.svg';

const useStyles = makeStyles((theme) => ({
  root: {
    flexGrow: 1,
  },
  menuButton: {
    marginRight: theme.spacing(2),
  },
  title: {
    flexGrow: 1,
  },
}));

const TopBar: FC = () => {
  const classes = useStyles();

  return (
    <div className={classes.root}>
      <AppBar position="static">
        <Toolbar>
          <IconButton
            edge="start"
            className={classes.menuButton}
            color="inherit"
          >
            <MenuIcon />
          </IconButton>
          <Typography
            variant="h6"
            className={classes.title}
          >
            Apex
          </Typography>
          {/* <img src={logo} className="App-logo" alt="logo" style={{ width: '200px' }} /> */}
        </Toolbar>
      </AppBar>
    </div>
  );
};

export default TopBar;
