import React, { FC } from 'react';
import {
  AppBar, Toolbar, Typography, makeStyles,
} from '@material-ui/core';
import MenuButton from './MenuButton';

const useStyles = makeStyles(() => ({
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
      <AppBar position="static" color="inherit">
        <Toolbar>
          <MenuButton />
          <Typography
            variant="h6"
            className={classes.title}
          >
            Djambi-N
          </Typography>
        </Toolbar>
      </AppBar>
    </div>
  );
};

export default TopBar;
