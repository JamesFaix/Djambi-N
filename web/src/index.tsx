import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore, applyMiddleware } from 'redux';
import { Router } from 'react-router-dom';
import thunk from 'redux-thunk';
import "./styles/styles.less";
import { ApiClientCore } from './api/clientCore';
import * as StoreRoot from './store/root';
import { SseClientManager } from './utilities/serverSentEvents';
import Copy from './utilities/copy';
import { createHashHistory } from 'history';
import Controller from './controllers/controller';

const store = createStore(
    StoreRoot.reducer,
    StoreRoot.defaultState,
    applyMiddleware(thunk)
);
const history = createHashHistory();

ApiClientCore.init(store);
SseClientManager.init(store);
Copy.init(store);
Controller.init(store, history);

render(
    <Provider store={store}>
        <Router history={history}>
            <App/>
        </Router>
    </Provider>,
    document.getElementById('root')
);