import React, { FC } from 'react';
import { Typography, Link } from '@material-ui/core';

const RulesPage: FC = () => {
  return (
    <div>
      <Typography variant="h4">
        Rules
      </Typography>
      <br />
      <Typography variant="body1">
        {'The rules are not currently embedded in the app, but can be found on the '}
        <Link href="https://github.com/JamesFaix/Djambi-N/wiki/Rules">wiki</Link>.
      </Typography>
    </div>
  );
};

export default RulesPage;
