import React, { FC } from 'react';
import {
  AppBar, Toolbar, Typography, makeStyles, Grid,
} from '@material-ui/core';
import MenuButton from './MenuButton';

const useStyles = makeStyles((theme) => ({
  root: {
    flexGrow: 1,
  },
  title: {
    flexGrow: 1,
    color: theme.palette.text.secondary,
    position: 'absolute',
    transform: 'translate(-50%, -50%)',
    margin: 0,
    top: '50%',
    left: '50%',
  },
}));

const TopBar: FC = () => {
  const classes = useStyles();

  return (
    <div className={classes.root}>
      <AppBar position="static" color="inherit">
        <Toolbar>
          <Grid container className={classes.root}>
            <Grid item xs style={{ display: 'flex' }}>
              <MenuButton />
            </Grid>
            <Grid item xs>
              <Typography
                variant="h6"
                className={classes.title}
              >
                Djambi-N
              </Typography>
            </Grid>
            <Grid item xs>
              {/* itentionally blank */}
            </Grid>
          </Grid>
        </Toolbar>
      </AppBar>
    </div>
  );
};

export default TopBar;
