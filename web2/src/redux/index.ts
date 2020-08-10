import { createStore, applyMiddleware } from 'redux';
import { logMiddleware } from './middleware';
import { rootReducer, defaultRootState } from './root';

export const store = createStore(
  rootReducer,
  defaultRootState,
  applyMiddleware(logMiddleware),
);
