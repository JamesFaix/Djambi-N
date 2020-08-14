import React, { FC } from 'react';
import {
  Table, TableContainer, TableHead, TableCell, TableRow, TableBody,
} from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectActiveGame, selectSession } from '../../hooks/selectors';
import {
  PlayerKind, PlayerDto, GameDto, UserDto,
} from '../../api-client';

function getPlayerNote(player: PlayerDto, game: GameDto): string {
  switch (player.kind) {
    case PlayerKind.Guest: {
      const host = game.players.find((p) => p.kind === PlayerKind.User
        && p.userId === player.userId);
      if (!host) { throw new Error('Host player for guest not found.'); }
      return `Guest of ${host.name}`;
    }
    case PlayerKind.Neutral:
      return 'Neutral';
    default:
      return '';
  }
}

enum LobbyPlayerActionType {
  Add,
  Remove,
  Quit
}

type PlayerSlotViewModel = {
  name: string,
  note: string,
  actionType: LobbyPlayerActionType | null
};

function getViewModels(currentUser: UserDto, game: GameDto): PlayerSlotViewModel[] {
  const players: PlayerSlotViewModel[] = game.players.map((p) => {
    const viewModel: PlayerSlotViewModel = {
      name: p.name,
      note: getPlayerNote(p, game),
      actionType: null,
    };

    if (p.userId === currentUser.id) {
      if (p.kind === PlayerKind.Guest) {
        // User can remove their own guests
        viewModel.actionType = LobbyPlayerActionType.Remove;
      } else if (p.kind === PlayerKind.User) {
        // User can quit
        viewModel.actionType = LobbyPlayerActionType.Quit;
      }
    } else if (game.createdBy.userId === currentUser.id) {
      // Game creator can remove any player
      viewModel.actionType = LobbyPlayerActionType.Remove;
    }

    return viewModel;
  });

  const emptySlotCount = game.parameters.regionCount - players.length;

  const emptySlots: PlayerSlotViewModel[] = [];

  for (let i = 0; i < emptySlotCount; i += 1) {
    emptySlots.push({
      name: '',
      note: '',
      actionType: LobbyPlayerActionType.Add,
    });
  }

  return players.concat(emptySlots);
}

const PendingLobbyPlayersTable: FC = () => {
  const { game } = useSelector(selectActiveGame);
  const { user } = useSelector(selectSession);

  if (game === null || user === null) {
    return <></>;
  }

  const viewModels = getViewModels(user, game);

  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Note</TableCell>
            <TableCell>Action</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {
            viewModels.map((p, i) => (
              <TableRow key={i.toString()}>
                <TableCell>{p.name}</TableCell>
                <TableCell>{p.note}</TableCell>
                <TableCell>{p.actionType}</TableCell>
              </TableRow>
            ))
          }
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default PendingLobbyPlayersTable;
