import React, { FC } from 'react';
import { List } from '@material-ui/core';
import {
  AccountBalance as DiplomacyIcon,
  CameraAlt as SnapshotsIcon,
  EmojiEvents as ResultsIcon,
  MeetingRoom as LobbyIcon,
  PlayArrow as PlayIcon,
} from '@material-ui/icons';
import { GameInfo } from '../../model/game';
import { navigateTo } from '../../utilities/navigation';
import NavigationItem from './NavigationItem';

const ActiveGameNavigationItems: FC<{ game: GameInfo }> = ({ game }) => {
  return (
    <List>
      <NavigationItem text="Game lobby" icon={<LobbyIcon />} onClick={() => navigateTo(`/games/${game.id}/lobby`)} />
      <NavigationItem text="Resume game" icon={<PlayIcon />} onClick={() => navigateTo(`/games/${game.id}/play`)} />
      <NavigationItem text="Game diplomacy" icon={<DiplomacyIcon />} onClick={() => navigateTo(`/games/${game.id}/diplomacy`)} />
      <NavigationItem text="Game snapshots" icon={<SnapshotsIcon />} onClick={() => navigateTo(`/games/${game.id}/snapshots`)} />
      <NavigationItem text="Game outcome" icon={<ResultsIcon />} onClick={() => navigateTo(`/games/${game.id}/outcome`)} />
    </List>
  );
};

export default ActiveGameNavigationItems;
