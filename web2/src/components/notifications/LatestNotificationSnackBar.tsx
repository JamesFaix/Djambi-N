import React, { FC } from 'react';
import { Snackbar } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectNotifications, selectConfig } from '../../hooks/selectors';
import NotificationAlert from './NotificationAlert';
import { hideNotificationSnackbar } from '../../controllers/notificationsController';

const LatestNotificationSnackbar: FC = () => {
  const { notifications, isSnackbarVisible } = useSelector(selectNotifications);
  const { user } = useSelector(selectConfig);

  if (notifications.length === 0) {
    return <></>;
  }

  const latest = notifications[0];

  const autoHideMs = user.notificationDisplaySeconds * 1000;

  return (
    <Snackbar
      autoHideDuration={autoHideMs}
      open={isSnackbarVisible}
      onClose={(e, reason) => {
        if (reason === 'clickaway') {
          return;
        }
        hideNotificationSnackbar();
      }}
    >
      <NotificationAlert notification={latest} />
    </Snackbar>
  );
};

export default LatestNotificationSnackbar;
