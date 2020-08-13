import React, { FC } from 'react';
import { ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import { navigateTo } from '../../utilities/navigation';

interface NavigationItemProps {
  text: string;
  icon: JSX.Element;
  path: string;
}

const NavigationItem: FC<NavigationItemProps> = ({ text, icon, path }) => {
  const onClick = () => {
    navigateTo(path);
  };

  return (
    <ListItem key={text} button onClick={onClick}>
      <ListItemIcon>{icon}</ListItemIcon>
      <ListItemText primary={text} />
    </ListItem>
  );
};

export default NavigationItem;
