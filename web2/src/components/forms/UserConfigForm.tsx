import React, { FC, useState, ChangeEvent } from 'react';
import {
  FormControl, FormLabel, Button, TextField, FormControlLabel, FormGroup, Checkbox,
} from '@material-ui/core';
import { setUserConfig } from '../../utilities/config';
import { UserConfig } from '../../model/configuration';

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

  const onLogReduxChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    logRedux: e.target.checked,
  });

  const onFavoriteWordChanged = (e: ChangeEvent<HTMLInputElement>) => setState({
    ...state,
    favoriteWord: e.target.value,
  });

  const onSubmitClicked = async () => {
    const config: UserConfig = {
      logRedux: state.logRedux,
      favoriteWord: state.favoriteWord,
    };

    await setUserConfig(config);
  };

  const controlStyle = {
    padding: '10px',
  };

  return (
    <div>
      <FormControl component="fieldset">
        <FormLabel>Settings</FormLabel>
        <FormGroup>
          <FormControlLabel
            checked={state.logRedux}
            label="Log Redux"
            labelPlacement="start"
            control={(
              <Checkbox
                onChange={onLogReduxChanged}
                style={controlStyle}
              />
            )}
          />
          <FormControlLabel
            value={state.favoriteWord}
            label="Favorite word"
            labelPlacement="start"
            control={(
              <TextField
                onChange={onFavoriteWordChanged}
                style={controlStyle}
              />
            )}
          />
          <Button
            onClick={onSubmitClicked}
            style={controlStyle}
          >
            Save
          </Button>
        </FormGroup>
      </FormControl>
    </div>
  );
};

export default UserConfigForm;
