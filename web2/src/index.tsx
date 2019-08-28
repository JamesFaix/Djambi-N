import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore, applyMiddleware } from 'redux';
import { Router } from 'react-router-dom';
import thunk from 'redux-thunk';
import {history} from './history';
import { logger } from 'redux-logger';
import "./index.css";
import "./styles/styles.less";
import Debug from './debug';
import { ApiClientCore } from './api/clientCore';
import { ApiRequest, ApiResponse, ApiError } from './api/requestModel';
import * as StoreApiClient from './store/apiClient';
import * as StoreRoot from './store/root';

const store = createStore(
    StoreRoot.reducer,
    StoreRoot.defaultState,
    Debug.logRedux ? applyMiddleware(thunk, logger) : applyMiddleware(thunk)
);

ApiClientCore.init(
    (request: ApiRequest) => store.dispatch(StoreApiClient.Actions.apiRequest(request)),
    (response: ApiResponse) => store.dispatch(StoreApiClient.Actions.apiResponse(response)),
    (error: ApiError) => store.dispatch(StoreApiClient.Actions.apiError(error)),
);

render(
    <Provider store={store}>
        <Router history={history}>
            <App/>
        </Router>
    </Provider>,
    document.getElementById('root')
);