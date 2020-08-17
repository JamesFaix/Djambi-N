import React, { FC, useState } from 'react';
import {
  FormControl, FormLabel, FormGroup,
} from '@material-ui/core';
import { signIn } from '../../controllers/userController';
import FormTextField from './controls/FormTextField';
import FormSubmitButton from './controls/FormSubmitButton';
import FormPasswordField from './controls/FormPasswordField';

type FormState = {
  username: string,
  password: string
};

const defaultState: FormState = {
  username: '',
  password: '',
};

const SignInForm: FC = () => {
  const [state, setState] = useState(defaultState);

  return (
    <div>
      <FormControl component="fieldset">
        <FormLabel>Sign in</FormLabel>
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
            value={state.password}
            onChanged={(e) => setState({
              ...state,
              password: e.target.value,
            })}
          />
          <br />
          <FormSubmitButton
            text="Submit"
            onClick={() => signIn({
              username: state.username,
              password: state.password,
            })}
          />
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default SignInForm;
