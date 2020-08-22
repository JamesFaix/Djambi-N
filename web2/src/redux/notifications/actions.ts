import { NotificationActionTypes } from './actionTypes';
import { Notification } from '../../model/notifications';

export type NotificationAddedAction = {
  type: typeof NotificationActionTypes.NotificationAdded,
  notification: Notification
};

export type NotificationRemovedAction = {
  type: typeof NotificationActionTypes.NotificationRemoved,
  id: string
};

export type NotificationAction =
  NotificationAddedAction |
  NotificationRemovedAction;
