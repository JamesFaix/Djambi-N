import React, { FC } from 'react';
import { Button, useTheme } from '@material-ui/core';
import { useFormStyles } from '../../../styles/styles';

interface FormSubmitButtonProps {
  onClick: () => void,
  text: string
}

const FormSubmitButton: FC<FormSubmitButtonProps> = ({ onClick, text }) => {
  const theme = useTheme();
  const styles = useFormStyles(theme);

  return (
    <Button
      onClick={onClick}
      className={styles.button}
    >
      {text}
    </Button>
  );
};

export default FormSubmitButton;
