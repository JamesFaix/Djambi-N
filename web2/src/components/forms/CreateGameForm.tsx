import React, { FC, useState, ChangeEvent } from 'react';
import {
  FormControl, FormLabel, Button, TextField, FormControlLabel, FormGroup, Checkbox,
} from '@material-ui/core';
import { createGame } from '../../controllers/gameController';

type FormState = {
  description: string | null,
  allowGuests: boolean,
  isPublic: boolean,
  regionCount: number
};

const defaultState: FormState = {
  description: null,
  allowGuests: true,
  isPublic: true,
  regionCount: 3,
};

const CreateGameForm: FC = () => {
  const [state, setState] = useState(defaultState);

  const onDescriptionChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    description: e.target.value,
  });

  const onAllowGuestsChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    allowGuests: e.target.checked,
  });

  const onIsPublicChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    isPublic: e.target.checked,
  });

  const onRegionCountChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    regionCount: Number(e.target.value),
  });

  const onSubmitClicked = () => {
    createGame({
      description: state.description,
      allowGuests: state.allowGuests,
      isPublic: state.isPublic,
      regionCount: state.regionCount,
    });
  };

  const controlStyle = {
    padding: '10px',
  };

  return (
    <div>
      <FormControl component="fieldset">
        <FormLabel>Create game</FormLabel>
        <FormGroup>
          <FormControlLabel
            value={state.description}
            label="Description"
            labelPlacement="start"
            control={(
              <TextField
                onChange={onDescriptionChanged}
                style={controlStyle}
              />
            )}
          />
          <FormControlLabel
            checked={state.allowGuests}
            label="Allow guest players"
            labelPlacement="start"
            control={(
              <Checkbox
                onChange={onAllowGuestsChanged}
                style={controlStyle}
              />
            )}
          />
          <FormControlLabel
            checked={state.isPublic}
            label="Public"
            labelPlacement="start"
            control={(
              <Checkbox
                onChange={onIsPublicChanged}
                style={controlStyle}
              />
            )}
          />
          <FormControlLabel
            value={state.regionCount}
            label="Board region count"
            labelPlacement="start"
            control={(
              <TextField
                type="number"
                onChange={onRegionCountChanged}
                style={controlStyle}
              />
            )}
          />
          <Button
            onClick={onSubmitClicked}
            style={controlStyle}
          >
            Submit
          </Button>
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default CreateGameForm;
