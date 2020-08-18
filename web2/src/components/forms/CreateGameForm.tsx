import React, { FC, useState } from 'react';
import { FormControl, FormGroup } from '@material-ui/core';
import { createGame } from '../../controllers/gameController';
import FormTextField from './controls/FormTextField';
import FormCheckbox from './controls/FormCheckbox';
import FormSubmitButton from './controls/FormSubmitButton';
import FormNumberField from './controls/FormNumberField';

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

const CreateGameForm: FC = () => {
  const [state, setState] = useState(defaultState);

  return (
    <div>
      <FormControl component="fieldset">
        <FormGroup>
          <FormTextField
            label="Description"
            value={state.description}
            onChanged={(e) => setState({
              ...state,
              description: e.target.value,
            })}
          />
          <FormCheckbox
            label="Allow guest players"
            value={state.allowGuests}
            onChanged={(e) => setState({
              ...state,
              allowGuests: e.target.checked,
            })}
          />
          <FormCheckbox
            label="Public"
            value={state.isPublic}
            onChanged={(e) => setState({
              ...state,
              isPublic: e.target.checked,
            })}
          />
          <FormNumberField
            label="Board region count"
            value={state.regionCount}
            onChanged={(e) => setState({
              ...state,
              regionCount: Number(e.target.value),
            })}
          />
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
