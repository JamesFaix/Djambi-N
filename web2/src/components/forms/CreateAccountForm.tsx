import React, { FC, useState } from 'react';
import {
  FormControl, FormLabel, FormGroup,
} from '@material-ui/core';
import { createAccount } from '../../controllers/userController';
import FormTextField from './controls/FormTextField';
import FormSubmitButton from './controls/FormSubmitButton';
import FormPasswordField from './controls/FormPasswordField';

type FormState = {
  username: string,
  password1: string,
  password2: string
};

const defaultState: FormState = {
  username: '',
  password1: '',
  password2: '',
};

const CreateAccountForm: FC = () => {
  const [state, setState] = useState(defaultState);

  const passwordsDontMatch = (state.password1 !== state.password2)
    && (state.password2 !== '');

  return (
    <div>
      <FormControl component="fieldset">
        <FormLabel>Create account</FormLabel>
        <FormGroup>
          <FormTextField
            label="Username"
            value={state.username}
            onChanged={(e) => setState({
              ...state,
              username: e.target.value,
            })}
          />
          <FormPasswordField
            label="Password"
            value={state.password1}
            onChanged={(e) => setState({
              ...state,
              password1: e.target.value,
            })}
            error={passwordsDontMatch}
            helperText={passwordsDontMatch ? 'Passwords do not match' : undefined}
          />
          <FormPasswordField
            label="Confirm password"
            value={state.password2}
            onChanged={(e) => setState({
              ...state,
              password2: e.target.value,
            })}
            error={passwordsDontMatch}
            helperText={passwordsDontMatch ? 'Passwords do not match' : undefined}
          />
          <br />
          <FormSubmitButton
            text="Submit"
            onClick={() => createAccount({
              name: state.username,
              password: state.password1,
            })}
          />
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default CreateAccountForm;
