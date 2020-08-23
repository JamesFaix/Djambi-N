import React, { FC } from 'react';
import { Typography, Container, List } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectNotifications } from '../../hooks/selectors';
import NotificationAlert from '../notifications/NotificationAlert';

const PageContent: FC = () => {
  const { notifications } = useSelector(selectNotifications);

  if (notifications.length === 0) {
    return (
      <Typography variant="body1">
        You do not have any notifications.
      </Typography>
    );
  }

  return (
    <List>
      {
        notifications.map((n, i) => (
          <NotificationAlert
            notification={n}
            key={i.toString()}
          />
        ))
      }
    </List>
  );
};

const NotificationsPage: FC = () => {
  return (
    <div>
      <Typography variant="h4">
        Notifications
      </Typography>
      <br />
      <Container maxWidth="sm">
        <PageContent />
      </Container>
    </div>
  );
};

export default NotificationsPage;
