import React from 'react';
import { render } from '@testing-library/react';
import { Provider } from 'react-redux';
import { store } from '../../redux';
import App from './App';

test('renders learn react link', () => {
  const { getByText } = render(
    <Provider store={store}>
      <App />
    </Provider>,
  );
  const linkElement = getByText(/API URL/i);
  expect(linkElement).toBeInTheDocument();
});
