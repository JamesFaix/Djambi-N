import { Theme, makeStyles } from '@material-ui/core';

// eslint-disable-next-line @typescript-eslint/explicit-module-boundary-types
export function useFormStyles(theme: Theme) {
  const useStyles = makeStyles({
    button: {
      border: '1px',
      borderStyle: 'solid',
      padding: '10px',
      width: '50%',
      alignSelf: 'center',
    },
    control: {
      padding: '10px',
    },
    label: {
      color: theme.palette.text.secondary,
    },
  });
  return useStyles();
}
