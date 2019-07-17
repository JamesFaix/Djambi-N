import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import configureStore from './store';

const store = configureStore();

render(
    <Provider store={store}>
        <App appState={store.getState()}/>
    </Provider>,
    document.getElementById('root')
);