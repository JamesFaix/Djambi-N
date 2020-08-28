import React, { FC, useState } from 'react';
import {
  FormControl,
  FormGroup,
  TableBody,
  Table,
  TableRow,
  Checkbox,
  TextField,
} from '@material-ui/core';
import { useSelector } from 'react-redux';
import { setUserConfig } from '../../controllers/configController';
import FormSubmitButton from './controls/FormSubmitButton';
import { selectConfig } from '../../hooks/selectors';
import { useFormStyles } from '../../styles/styles';
import { theme } from '../../styles/materialTheme';
import FormTableCell from './controls/FormTableCell';

const TableCell = FormTableCell;

const UserConfigForm: FC = () => {
  const { user } = useSelector(selectConfig);
  const [state, setState] = useState(user);
  const styles = useFormStyles(theme);

  return (
    <FormControl component="fieldset">
      <FormGroup>
        <Table>
          <TableBody>
            <TableRow>
              <TableCell>Log Redux</TableCell>
              <TableCell>
                <Checkbox
                  className={styles.control}
                  checked={state.logRedux}
                  onChange={(e) => setState({
                    ...state,
                    logRedux: e.target.checked,
                  })}
                />
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell>Seconds to display notifications</TableCell>
              <TableCell>
                <TextField
                  type="number"
                  className={styles.control}
                  value={state.notificationDisplaySeconds}
                  onChange={(e) => setState({
                    ...state,
                    notificationDisplaySeconds: Number(e.target.value),
                  })}
                />
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
        <br />
        <FormSubmitButton
          text="Save"
          onClick={() => setUserConfig(state)}
        />
      </FormGroup>
    </FormControl>
  );
};

export default UserConfigForm;
