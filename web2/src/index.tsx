import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore, applyMiddleware } from 'redux';
import { reducer } from './store/reducers';
import { defaultState } from './store/state';
import { Router } from 'react-router-dom';
import thunk from 'redux-thunk';
import {history} from './history';
import { logger } from 'redux-logger';
import "./index.css";

const store = createStore(
    reducer,
    defaultState(),
    applyMiddleware(thunk, logger)
);

render(
    <Provider store={store}>
        <Router history={history}>
            <App/>
        </Router>
    </Provider>,
    document.getElementById('root')
);