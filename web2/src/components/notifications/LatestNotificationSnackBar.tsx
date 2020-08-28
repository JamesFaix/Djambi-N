import React, { FC, useState } from 'react';
import { Snackbar } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectNotifications, selectConfig } from '../../hooks/selectors';
import NotificationAlert from './NotificationAlert';

const LatestNotificationSnackbar: FC = () => {
  const { notifications } = useSelector(selectNotifications);
  const { user } = useSelector(selectConfig);
  const [isOpen, setIsOpen] = useState(true);

  if (notifications.length === 0) {
    return <></>;
  }

  const latest = notifications[0];
  const autoHideMs = user.notificationDisplaySeconds * 1000;

  return (
    <Snackbar
      // autoHideDuration={autoHideMs}
      open={isOpen}
      onClose={(e, reason) => {
        if (reason === 'clickaway') {
          return;
        }
        setIsOpen(false);
      }}
    >
      <NotificationAlert notification={latest} />
    </Snackbar>
  );
};

export default LatestNotificationSnackbar;
