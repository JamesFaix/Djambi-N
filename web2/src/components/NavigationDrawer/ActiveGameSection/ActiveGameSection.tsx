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
import NavigationItem from '../NavigationItem';
import { sectionHeaderStyle } from '../styles';
import { GameStatus } from '../../../api-client';
import * as Routes from '../../../utilities/routes';

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
        >
          {`Game ${game.id}`}

        </Typography>

        {game.description ? (
          <Typography variant="h6" className={headerStyle.h6}>
            {game.description}
          </Typography>
        ) : undefined}

        <NavigationItem
          text="Lobby"
          icon={<LobbyIcon />}
          path={Routes.gameLobby(game.id)}
        />

        {[GameStatus.InProgress, GameStatus.Over].includes(game.status) ? (
          <NavigationItem
            text="Resume"
            icon={<PlayIcon />}
            path={Routes.gamePlay(game.id)}
          />
        ) : undefined}

        {game.status === GameStatus.InProgress ? (
          <NavigationItem
            text="Diplomacy"
            icon={<DiplomacyIcon />}
            path={Routes.gameDiplomacy(game.id)}
          />
        ) : undefined}

        {/* Only if user has privilege */}
        <NavigationItem
          text="Snapshots"
          icon={<SnapshotsIcon />}
          path={Routes.gameSnapshots(game.id)}
        />

        {game.status === GameStatus.Over ? (
          <NavigationItem
            text="Outcome"
            icon={<ResultsIcon />}
            path={Routes.gameOutcome(game.id)}
          />
        ) : undefined}
      </List>
    </>
  );
};

export default ActiveGameSection;
