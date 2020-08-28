import { makeStyles } from '@material-ui/core';
import { theme } from '../../styles/materialTheme';

export const topBarStyles = makeStyles({
  button: {
    marginRight: theme.spacing(2),
    color: theme.palette.text.secondary,
  },
});
