import { NotificationsState, defaultNotificationsState } from './state';
import { NotificationAction } from './actions';
import { NotificationActionTypes } from './actionTypes';
import { Notification } from '../../model/notifications';

function sortByTimeDescending(notifications: Notification[]): Notification[] {
  return notifications.sort((x, y) => {
    if (x.time < y.time) {
      return 1;
    }
    if (x.time > y.time) {
      return -1;
    }
    return 0;
  });
}

export function notificationsReducer(
  state: NotificationsState = defaultNotificationsState,
  action: NotificationAction,
): NotificationsState {
  switch (action.type) {
    case NotificationActionTypes.NotificationAdded:
      return {
        ...state,
        notifications: sortByTimeDescending(
          [...state.notifications, action.notification],
        ),
      };

    case NotificationActionTypes.NotificationRemoved:
      return {
        ...state,
        notifications: sortByTimeDescending(
          state.notifications.filter((n) => n.id !== action.id),
        ),
      };

    case NotificationActionTypes.ShowSnackbar:
      return {
        ...state,
        isSnackbarVisible: true,
      };

    case NotificationActionTypes.HideSnackbar:
      return {
        ...state,
        isSnackbarVisible: false,
      };

    default:
      return state;
  }
}
