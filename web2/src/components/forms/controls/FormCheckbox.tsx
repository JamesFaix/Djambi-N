import React, { FC, ChangeEvent } from 'react';
import {
  FormControlLabel, useTheme, Checkbox,
} from '@material-ui/core';
import { useFormStyles } from '../../../styles/styles';

interface Props {
  value: boolean,
  label: string,
  onChanged: (e: ChangeEvent<HTMLInputElement>) => void,
}

const FormCheckbox: FC<Props> = ({
  value, label, onChanged,
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
        <Checkbox
          className={styles.control}
          onChange={onChanged}
        />
      )}
    />
  );
};

export default FormCheckbox;
