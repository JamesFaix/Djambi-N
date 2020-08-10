import { Dispatch, MiddlewareAPI } from 'redux';
import { RootAction } from './root';
import * as Log from '../utilities/log';

// eslint-disable-next-line  max-len
export const logMiddleware = (store: MiddlewareAPI) => (next: Dispatch) => (action: RootAction): RootAction => {
  const oldState = store.getState();
  const shouldLog = oldState.config.user.logRedux as boolean;

  if (shouldLog) {
    Log.info('dispatching', action);
  }

  const result = next(action);

  if (shouldLog) {
    const newState = store.getState();
    Log.info('next state', newState);
  }

  return result;
};
