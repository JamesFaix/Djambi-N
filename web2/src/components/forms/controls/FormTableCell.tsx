import {
  TableCell,
  withStyles,
} from '@material-ui/core';

const FormTableCell = withStyles({
  root: {
    borderBottom: 'none',
  },
})(TableCell);

export default FormTableCell;
