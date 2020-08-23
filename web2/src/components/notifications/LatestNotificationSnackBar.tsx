import React, { FC } from 'react';
import { Snackbar } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectNotifications } from '../../hooks/selectors';
import NotificationAlert from './NotificationAlert';

const LatestNotificationSnackbar: FC = () => {
  const { notifications } = useSelector(selectNotifications);

  if (notifications.length === 0) {
    return <></>;
  }

  const latest = notifications[0];

  return (
    <Snackbar
      autoHideDuration={6000}
      open
    >
      <NotificationAlert notification={latest} />
    </Snackbar>
  );
};

export default LatestNotificationSnackbar;
