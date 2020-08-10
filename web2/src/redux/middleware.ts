import { Dispatch, MiddlewareAPI } from 'redux';
import { RootAction } from './root';
import * as Log from '../utilities/log';

// eslint-disable-next-line  max-len
export const logMiddleware = (store: MiddlewareAPI) => (next: Dispatch) => (action: RootAction): RootAction => {
  Log.info('dispatching', action);
  const result = next(action);
  Log.info('next state', store.getState());
  return result;
};
