import React from 'react';
import { render } from '@testing-library/react';
import { Provider } from 'react-redux';
import { store } from '../../redux';
import App from './App';

describe('<App/>', () => {
  it('renders', () => {
    render(
      <Provider store={store}>
        <App />
      </Provider>,
    );
  });
});
