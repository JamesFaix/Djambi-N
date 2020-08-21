import { navigateTo as createNavigateToAction, openDrawer, closeDrawer } from '../redux/navigation/actionFactory';
import { store } from '../redux';

export async function navigateTo(path: string): Promise<void> {
  const action = createNavigateToAction(path);
  store.dispatch(action);
}

export async function toggleDrawer(isOpen: boolean): Promise<void> {
  const action = isOpen ? openDrawer() : closeDrawer();
  store.dispatch(action);
}
