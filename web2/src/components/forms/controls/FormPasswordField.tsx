import React, { FC, ChangeEvent } from 'react';
import { FormControlLabel, TextField, useTheme } from '@material-ui/core';
import { useFormStyles } from '../../../styles/styles';

interface Props {
  value: string,
  label: string,
  onChanged: (e: ChangeEvent<HTMLInputElement>) => void,
  error?: boolean,
  helperText?: string
}

const FormPasswordField: FC<Props> = ({
  value, label, onChanged, error, helperText,
}) => {
  const theme = useTheme();
  const styles = useFormStyles(theme);

  return (
    <FormControlLabel
      value={value}
      label={label}
      labelPlacement="start"
      className={styles.label}
      control={(
        <TextField
          className={styles.control}
          onChange={onChanged}
          error={error}
          helperText={helperText}
          type="password"
        />
      )}
    />
  );
};

export default FormPasswordField;
