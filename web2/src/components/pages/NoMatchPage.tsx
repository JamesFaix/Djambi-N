import React, { FC } from 'react';
import { Typography } from '@material-ui/core';

const NoMatchPage: FC = () => {
  return (
    <div>
      <Typography variant="h4">
        Page not found
      </Typography>
    </div>
  );
};

export default NoMatchPage;
