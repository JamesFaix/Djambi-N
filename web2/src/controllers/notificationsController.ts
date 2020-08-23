import { store } from '../redux';
import * as ActionFactory from '../redux/notifications/actionFactory';

export async function removeNotification(id: string): Promise<void> {
  const action = ActionFactory.removeNotification(id);
  store.dispatch(action);
}
