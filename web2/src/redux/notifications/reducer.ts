import { NotificationsState, defaultNotificationsState } from './state';
import { NotificationAction } from './actions';
import { NotificationActionTypes } from './actionTypes';

export function notificationsReducer(
  state: NotificationsState = defaultNotificationsState,
  action: NotificationAction,
): NotificationsState {
  switch (action.type) {
    case NotificationActionTypes.NotificationAdded:
      return {
        ...state,
        notifications: [...state.notifications, action.notification],
      };

    case NotificationActionTypes.NotificationRemoved:
      return {
        ...state,
        notifications: state.notifications.filter((n) => n.id !== action.id),
      };

    default:
      return state;
  }
}
