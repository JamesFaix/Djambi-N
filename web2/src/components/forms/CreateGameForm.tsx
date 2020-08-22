import React, { FC, useState } from 'react';
import {
  FormControl,
  FormGroup,
  Table,
  TableBody,
  TableRow,
  TableCell as MuiTableCell,
  TextField,
  Checkbox,
  withStyles,
} from '@material-ui/core';
import { createGame } from '../../controllers/gameController';
import FormSubmitButton from './controls/FormSubmitButton';

type FormState = {
  description: string,
  allowGuests: boolean,
  isPublic: boolean,
  regionCount: number
};

const defaultState: FormState = {
  description: '',
  allowGuests: true,
  isPublic: true,
  regionCount: 3,
};

const TableCell = withStyles({
  root: {
    borderBottom: 'none',
  },
})(MuiTableCell);

const CreateGameForm: FC = () => {
  const [state, setState] = useState(defaultState);

  return (
    <div>
      <FormControl component="fieldset">
        <FormGroup>
          <Table>
            <TableBody>
              <TableRow>
                <TableCell>
                  Description
                </TableCell>
                <TableCell>
                  <TextField
                    value={state.description}
                    onChange={(e) => setState({
                      ...state,
                      description: e.target.value,
                    })}
                  />
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell>
                  Allow guest players
                </TableCell>
                <TableCell>
                  <Checkbox
                    checked={state.allowGuests}
                    onChange={(e) => setState({
                      ...state,
                      allowGuests: e.target.checked,
                    })}
                  />
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell>
                  Public
                </TableCell>
                <TableCell>
                  <Checkbox
                    checked={state.isPublic}
                    onChange={(e) => setState({
                      ...state,
                      isPublic: e.target.checked,
                    })}
                  />
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell>
                  Board regions
                </TableCell>
                <TableCell>
                  <TextField
                    type="number"
                    value={state.regionCount}
                    onChange={(e) => setState({
                      ...state,
                      regionCount: Number(e.target.value),
                    })}
                  />
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
          <br />
          <FormSubmitButton
            text="Submit"
            onClick={() => createGame({
              description: state.description,
              allowGuests: state.allowGuests,
              isPublic: state.isPublic,
              regionCount: state.regionCount,
            })}
          />
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default CreateGameForm;
