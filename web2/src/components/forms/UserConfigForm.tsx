import React, { FC, useState } from 'react';
import { FormControl, FormGroup } from '@material-ui/core';
import { setUserConfig } from '../../controllers/configController';
import FormTextField from './controls/FormTextField';
import FormSubmitButton from './controls/FormSubmitButton';
import FormCheckbox from './controls/FormCheckbox';

type FormState = {
  logRedux: boolean,
  favoriteWord: string
};

const defaultState: FormState = {
  logRedux: false,
  favoriteWord: 'scrupulous',
};

const UserConfigForm: FC = () => {
  const [state, setState] = useState(defaultState);

  return (
    <div>
      <FormControl component="fieldset">
        <FormGroup>
          <FormCheckbox
            label="Log Redux"
            value={state.logRedux}
            onChanged={(e) => setState({
              ...state,
              logRedux: e.target.checked,
            })}
          />
          <FormTextField
            label="Favorite word"
            value={state.favoriteWord}
            onChanged={(e) => setState({
              ...state,
              favoriteWord: e.target.value,
            })}
          />
          <br />
          <FormSubmitButton
            text="Save"
            onClick={() => setUserConfig({
              logRedux: state.logRedux,
              favoriteWord: state.favoriteWord,
            })}
          />
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default UserConfigForm;
