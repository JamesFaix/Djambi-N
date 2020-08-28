import { store } from '../redux';
import * as ActionFactory from '../redux/notifications/actionFactory';
import { Notification } from '../model/notifications';

export async function showNotificationSnackbar(): Promise<void> {
  const action = ActionFactory.showNotificationSnackbar();
  store.dispatch(action);
}

export async function hideNotificationSnackbar(): Promise<void> {
  const action = ActionFactory.hideNotificationSnackbar();
  store.dispatch(action);
}

export async function addNotification(notification: Notification): Promise<void> {
  const action = ActionFactory.addNotification(notification);
  store.dispatch(action);
  showNotificationSnackbar();
}

export async function removeNotification(id: string): Promise<void> {
  const action = ActionFactory.removeNotification(id);
  store.dispatch(action);
}
