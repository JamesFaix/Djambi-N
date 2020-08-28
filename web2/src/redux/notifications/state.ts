import { Notification } from '../../model/notifications';

export type NotificationsState = {
  notifications: Notification[],
  isSnackbarVisible: boolean,
};

export const defaultNotificationsState: NotificationsState = {
  notifications: [],
  isSnackbarVisible: true,
};
