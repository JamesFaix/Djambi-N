import { navigateTo as createNavigateToAction } from '../redux/navigation/actionFactory';
import { store } from '../redux';

export async function navigateTo(path: string): Promise<void> {
  const action = createNavigateToAction(path);
  store.dispatch(action);
}
