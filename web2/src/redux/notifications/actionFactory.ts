import { Notification } from '../../model/notifications';
import {
  NotificationAddedAction,
  NotificationRemovedAction,
  ShowNotificationSnackbarAction,
  HideNotificationSnackbarAction,
} from './actions';
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

export function showNotificationSnackbar(): ShowNotificationSnackbarAction {
  return {
    type: NotificationActionTypes.ShowSnackbar,
  };
}

export function hideNotificationSnackbar(): HideNotificationSnackbarAction {
  return {
    type: NotificationActionTypes.HideSnackbar,
  };
}
