import React, { FC } from 'react';
import { List, Typography, Divider } from '@material-ui/core';
import {
  AccountBalance as DiplomacyIcon,
  CameraAlt as SnapshotsIcon,
  EmojiEvents as ResultsIcon,
  MeetingRoom as LobbyIcon,
  PlayArrow as PlayIcon,
  Info as InfoIcon,
} from '@material-ui/icons';
import { useSelector } from 'react-redux';
import NavigationItem from '../NavigationItem';
import { sectionHeaderStyle } from '../styles';
import { GameStatus } from '../../../api-client';
import * as Routes from '../../../utilities/routes';
import { selectActiveGame } from '../../../hooks/selectors';

const ActiveGameSection: FC = () => {
  const headerStyle = sectionHeaderStyle();
  const activeGame = useSelector(selectActiveGame);
  const { game } = activeGame;

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

        {game.parameters.description ? (
          <Typography variant="h6" className={headerStyle.h6}>
            {game.parameters.description}
          </Typography>
        ) : undefined}

        {game.status === GameStatus.Pending ? (
          <NavigationItem
            text="Lobby"
            icon={<LobbyIcon />}
            path={Routes.gameLobby(game.id)}
          />
        )
          : (
            <NavigationItem
              text="Info"
              icon={<InfoIcon />}
              path={Routes.gameInfo(game.id)}
            />
          )}

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
