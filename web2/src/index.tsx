import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore } from 'redux';
import { reducer } from './store/reducers';
import ApiClient from './api/client';
import { ActionFactory } from './store/actions';
import { Repository } from './repository';
import { defaultState } from './store/state';
import { HashRouter } from 'react-router-dom';

const store = createStore(reducer, defaultState());
const api = new ApiClient();
const actionFactory = new ActionFactory();
const repo = new Repository(api, store, actionFactory);

render(
    <Provider store={store}>
        <HashRouter>
            <App
                appState={store.getState()}
                repo={repo}
            />
        </HashRouter>
    </Provider>,
    document.getElementById('root')
);