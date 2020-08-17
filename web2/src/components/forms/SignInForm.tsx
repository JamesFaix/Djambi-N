import React, { FC, useState, ChangeEvent } from 'react';
import {
  FormControl, FormLabel, Button, TextField, FormControlLabel, FormGroup, useTheme,
} from '@material-ui/core';
import { signIn } from '../../controllers/userController';
import { useFormStyles } from '../../styles/styles';

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
  const theme = useTheme();
  const styles = useFormStyles(theme);

  const onUsernameChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    username: e.target.value,
  });

  const onPasswordChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    password: e.target.value,
  });

  const onSubmitClicked = () => {
    signIn({
      username: state.username,
      password: state.password,
    });
  };

  const controlStyle = {
    padding: '10px',
  };

  return (
    <div>
      <FormControl component="fieldset">
        <FormLabel>Sign in</FormLabel>
        <FormGroup>
          <FormControlLabel
            value={state.username}
            label="Username"
            labelPlacement="start"
            control={(
              <TextField
                onChange={onUsernameChanged}
                style={controlStyle}
              />
            )}
          />
          <FormControlLabel
            value={state.password}
            label="Password"
            labelPlacement="start"
            control={(
              <TextField
                type="password"
                onChange={onPasswordChanged}
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

export default SignInForm;
