import { useSelector } from 'react-redux';
import React, { FC } from 'react';
import { useLocation, Redirect } from 'react-router-dom';
import { selectNavigation } from '../../hooks/selectors';

const RedirectBasedOnStore: FC = () => {
  const navigation = useSelector(selectNavigation);
  const location = useLocation();

  if (navigation.path !== location.pathname) {
    return (
      <Redirect to={navigation.path} />
    );
  }

  return <></>;
};

export default RedirectBasedOnStore;
