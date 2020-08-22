import { Notification } from '../../model/notifications';

export type NotificationsState = {
  notifications: Notification[]
};

export const defaultNotificationsState: NotificationsState = {
  notifications: [],
};
