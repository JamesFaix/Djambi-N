import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore, applyMiddleware } from 'redux';
import { reducer } from './store/reducers';
import { defaultState } from './store/state';
import { HashRouter } from 'react-router-dom';
import thunk from 'redux-thunk';

const store = createStore(
    reducer,
    defaultState(),
    applyMiddleware(thunk),
);

render(
    <Provider store={store}>
        <HashRouter>
            <App/>
        </HashRouter>
    </Provider>,
    document.getElementById('root')
);