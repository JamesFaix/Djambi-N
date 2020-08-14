import React, { FC } from 'react';
import {
  AppBar, Toolbar, Typography, makeStyles,
} from '@material-ui/core';
import logo from '../../assets/logo.svg';
import MenuButton from './MenuButton';

const useStyles = makeStyles((theme) => ({
  root: {
    flexGrow: 1,
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
          <MenuButton />
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
