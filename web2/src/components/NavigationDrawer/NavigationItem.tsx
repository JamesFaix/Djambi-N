import React, { FC } from 'react';
import { ListItem, ListItemIcon, ListItemText } from '@material-ui/core';

interface NavigationItemProps {
  text: string;
  icon: JSX.Element;
  onClick: () => void;
}

const NavigationItem: FC<NavigationItemProps> = ({ text, icon, onClick }) => (
  <ListItem button key={text} onClick={onClick}>
    <ListItemIcon>{icon}</ListItemIcon>
    <ListItemText primary={text} />
  </ListItem>
);

export default NavigationItem;
