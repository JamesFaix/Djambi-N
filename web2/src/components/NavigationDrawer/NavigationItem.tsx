import React, { FC } from 'react';
import { ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import { Link } from 'react-router-dom';

interface NavigationItemProps {
  text: string;
  icon: JSX.Element;
  path: string;
}

const NavigationItem: FC<NavigationItemProps> = ({ text, icon, path }) => (
  <Link to={path}>
    <ListItem key={text}>
      <ListItemIcon>{icon}</ListItemIcon>
      <ListItemText primary={text} />
    </ListItem>
  </Link>
);

export default NavigationItem;
