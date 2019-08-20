import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore, applyMiddleware } from 'redux';
import { reducer } from './store/reducers';
import { Router } from 'react-router-dom';
import thunk from 'redux-thunk';
import {history} from './history';
import { logger } from 'redux-logger';
import { StateFactory } from './store/state';
import "./index.css";
import Debug from './debug';

const store = createStore(
    reducer,
    StateFactory.defaultAppState(),
    Debug.logRedux ? applyMiddleware(thunk, logger) : applyMiddleware(thunk)
);

render(
    <Provider store={store}>
        <Router history={history}>
            <App/>
        </Router>
    </Provider>,
    document.getElementById('root')
);