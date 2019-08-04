import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore } from 'redux';
import { reducer } from './store/reducers';
import ApiClient from './api/client';
import { ActionFactory } from './store/actions';
import { Repository } from './repository';

const store = createStore(reducer);
const api = new ApiClient();
const actionFactory = new ActionFactory();
const repo = new Repository(api, store, actionFactory);

render(
    <Provider store={store}>
        <App appState={store.getState()}/>
    </Provider>,
    document.getElementById('root')
);