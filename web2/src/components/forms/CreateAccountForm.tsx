import React, { FC, useState, ChangeEvent } from 'react';
import {
  FormControl, FormLabel, FormGroup,
} from '@material-ui/core';
import { createAccount } from '../../controllers/userController';
import FormTextField from './controls/FormTextField';
import FormSubmitButton from './controls/FormSubmitButton';

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

  const onUsernameChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    username: e.target.value,
  });

  const onPassword1Changed = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    password1: e.target.value,
  });

  const onPassword2Changed = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    password2: e.target.value,
  });

  const onSubmitClicked = () => {
    createAccount({
      name: state.username,
      password: state.password1,
    });
  };

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
            onChanged={onUsernameChanged}
          />
          <FormTextField
            label="Password"
            value={state.password1}
            onChanged={onPassword1Changed}
            error={passwordsDontMatch}
            helperText={passwordsDontMatch ? 'Passwords do not match' : undefined}
          />
          <FormTextField
            label="Confirm password"
            value={state.password2}
            onChanged={onPassword2Changed}
            error={passwordsDontMatch}
            helperText={passwordsDontMatch ? 'Passwords do not match' : undefined}
          />
          <br />
          <FormSubmitButton
            text="Submit"
            onClick={onSubmitClicked}
          />
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default CreateAccountForm;
