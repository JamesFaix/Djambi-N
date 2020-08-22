import { Notification } from '../../model/notifications';
import { NotificationAddedAction, NotificationRemovedAction } from './actions';
import { NotificationActionTypes } from './actionTypes';

export function addNotification(notification: Notification): NotificationAddedAction {
  return {
    type: NotificationActionTypes.NotificationAdded,
    notification,
  };
}

export function removeNotification(id: string): NotificationRemovedAction {
  return {
    type: NotificationActionTypes.NotificationRemoved,
    id,
  };
}
