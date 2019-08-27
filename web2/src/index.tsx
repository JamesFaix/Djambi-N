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
import { ApiClientCore } from './api/clientCore';
import { ApiRequest, ApiResponse, ApiError } from './api/requestModel';
import * as Actions from './store/actions';

const store = createStore(
    reducer,
    StateFactory.defaultAppState(),
    Debug.logRedux ? applyMiddleware(thunk, logger) : applyMiddleware(thunk)
);

ApiClientCore.init(
    (request: ApiRequest) => store.dispatch(Actions.apiRequest(request)),
    (response: ApiResponse) => store.dispatch(Actions.apiResponse(response)),
    (error: ApiError) => store.dispatch(Actions.apiError(error)),
);

render(
    <Provider store={store}>
        <Router history={history}>
            <App/>
        </Router>
    </Provider>,
    document.getElementById('root')
);