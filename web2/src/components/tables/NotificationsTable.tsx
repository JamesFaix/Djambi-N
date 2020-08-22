import React, { FC } from 'react';
import {
  Table,
  TableContainer,
  TableCell,
  TableRow,
  TableBody,
} from '@material-ui/core';
import { Alert } from '@material-ui/lab';
import { Notification, NotificationLevel } from '../../model/notifications';

interface Props {
  notifications: Notification[]
}

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

const NotificationsTable: FC<Props> = ({ notifications }) => {
  return (
    <TableContainer>
      <Table>
        <TableBody>
          {
            notifications.map((n, i) => (
              <TableRow key={i.toString()}>
                <TableCell>
                  <Alert severity={getAlertSeverity(n.level)}>{n.message}</Alert>
                </TableCell>
              </TableRow>
            ))
          }
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default NotificationsTable;
