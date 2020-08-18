import React, { FC } from 'react';
import { Typography } from '@material-ui/core';
import UserConfigForm from '../forms/UserConfigForm';

const UserConfigPage: FC = () => {
  return (
    <div>
      <Typography variant="h4">
        Settings
      </Typography>
      <br />
      <UserConfigForm />
    </div>
  );
};

export default UserConfigPage;
