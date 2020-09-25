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

const playerColors = new Map<number, string>([
  [0, 'blue'],
  [1, 'red'],
  [2, 'green'],
  [3, 'orange'],
  [4, 'brown'],
  [5, 'teal'],
  [6, 'magenta'],
  [7, 'gold'],
]);

export const pieceColors = {
  placeholder: '#CCCC33',
  getPlayer: (colorId: number | null) : string => {
    if (colorId === null) { return '#555555'; }
    const color = playerColors.get(colorId);
    if (!color) { throw Error(`Invalid colorID: ${colorId}`); }
    return color;
  },
};
