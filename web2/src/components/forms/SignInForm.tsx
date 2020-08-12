import React, { FC, useState, ChangeEvent } from 'react';
import {
  FormControl, FormLabel, Button, TextField, FormControlLabel, FormGroup,
} from '@material-ui/core';
import { useDispatch } from 'react-redux';
import { apiService } from '../../utilities/apiService';
import { ApiSessionsPostRequest } from '../../api-client';
import { loggedIn } from '../../redux/session/actionFactory';

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
  const dispatch = useDispatch();

  const onUsernameChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    username: e.target.value,
  });

  const onPasswordChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    password: e.target.value,
  });

  const onSubmitClicked = async () => {
    const loginParams: ApiSessionsPostRequest = {
      loginRequestDto: {
        username: state.username,
        password: state.password,
      },
    };
    const session = await apiService.sessions.apiSessionsPost(loginParams);

    const action = loggedIn(session.user);
    dispatch(action);
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
