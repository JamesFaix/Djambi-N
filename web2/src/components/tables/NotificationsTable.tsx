import React, { FC } from 'react';
import { List } from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import { Notification, NotificationLevel } from '../../model/notifications';
import { removeNotification } from '../../controllers/notificationsController';

type AlertSeverity = 'info' | 'success' | 'warning' | 'error' | undefined;

function getAlertSeverity(level: NotificationLevel): AlertSeverity {
  switch (level) {
    case NotificationLevel.Info:
      return 'info';
    case NotificationLevel.Warning:
      return 'warning';
    case NotificationLevel.Error:
      return 'error';
    default:
      return undefined;
  }
}

interface AlertProps {
  notification: Notification
}

const NotificationAlert: FC<AlertProps> = ({ notification }) => {
  const messages = notification.message.split('\n');

  return (
    <Alert
      severity={getAlertSeverity(notification.level)}
      onClose={() => removeNotification(notification.id)}
    >
      {messages.map((m, i) => (
        <div
          key={i.toString()}
          style={{ textAlign: 'left' }}
        >
          {m}
        </div>
      ))}
    </Alert>
  );
};

interface ListProps {
  notifications: Notification[]
}

const NotificationsTable: FC<ListProps> = ({ notifications }) => {
  return (
    <List>
      {
        notifications.map((n, i) => (
          <NotificationAlert notification={n} key={i.toString()} />
        ))
      }
    </List>
  );
};

export default NotificationsTable;
