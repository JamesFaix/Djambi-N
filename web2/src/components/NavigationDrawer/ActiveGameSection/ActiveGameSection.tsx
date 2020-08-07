import React, { FC } from 'react';
import { List, Typography, Divider } from '@material-ui/core';
import {
  AccountBalance as DiplomacyIcon,
  CameraAlt as SnapshotsIcon,
  EmojiEvents as ResultsIcon,
  MeetingRoom as LobbyIcon,
  PlayArrow as PlayIcon,
} from '@material-ui/icons';
import { GameInfo } from '../../../model/game';
import { navigateTo } from '../../../utilities/navigation';
import NavigationItem from '../NavigationItem';
import { sectionHeaderStyle } from '../styles';
import { GameStatus } from '../../../api-client';

const ActiveGameSection: FC<{ game: GameInfo | null }> = ({ game }) => {
  const headerStyle = sectionHeaderStyle();

  if (game === null) {
    return <></>;
  }

  return (
    <>
      <Divider />
      <List>
        <Typography
          variant="h5"
          className={headerStyle.h5}
        >{`Game ${game.id}`}</Typography>

        {game.description ? (
          <Typography variant="h6" className={headerStyle.h6}>
            {game.description}
          </Typography>
        ) : undefined}

        <NavigationItem
          text="Lobby"
          icon={<LobbyIcon />}
          onClick={() => navigateTo(`/games/${game.id}/lobby`)}
        />

        {[GameStatus.InProgress, GameStatus.Over].includes(game.status) ? (
          <NavigationItem
            text="Resume"
            icon={<PlayIcon />}
            onClick={() => navigateTo(`/games/${game.id}/play`)}
          />
        ) : undefined}

        {game.status === GameStatus.InProgress ? (
          <NavigationItem
            text="Diplomacy"
            icon={<DiplomacyIcon />}
            onClick={() => navigateTo(`/games/${game.id}/diplomacy`)}
          />
        ) : undefined}

        {/* Only if user has privilege */}
        <NavigationItem
          text="Snapshots"
          icon={<SnapshotsIcon />}
          onClick={() => navigateTo(`/games/${game.id}/snapshots`)}
        />

        {game.status === GameStatus.Over ? (
          <NavigationItem
            text="Outcome"
            icon={<ResultsIcon />}
            onClick={() => navigateTo(`/games/${game.id}/outcome`)}
          />
        ) : undefined}
      </List>
    </>
  );
};

export default ActiveGameSection;
