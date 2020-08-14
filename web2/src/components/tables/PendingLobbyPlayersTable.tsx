import React, { FC, useState } from 'react';
import {
  Table, TableContainer, TableHead, TableCell, TableRow, TableBody, TextField,
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

interface RowProps {
  player: PlayerSlotViewModel,
  game: GameDto
}

const Row: FC<RowProps> = ({ player, game }) => {
  const controlStyle = {
    padding: '10px',
  };

  const [name, setName] = useState('');

  let helperText = '';

  const isDuplicateName = game.players.find((p) => p.name === name);
  if (isDuplicateName) {
    helperText = `There is already a player named ${name}`;
  }

  const isBlank = name === '';
  if (isBlank) {
    helperText = 'Name cannot be blank';
  }

  const isValidName = !isDuplicateName && !isBlank;

  return (
    <TableRow>
      <TableCell>{
        player.actionType === LobbyPlayerActionType.Add
          ? (
            <TextField
              value={name}
              onChange={(e) => setName(e.target.value)}
              error={!isValidName}
              helperText={helperText}
              style={controlStyle}
            />
          )
          : player.name
      }
      </TableCell>
      <TableCell>{player.note}</TableCell>
      <TableCell>{player.actionType}</TableCell>
    </TableRow>
  );
};

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
            viewModels.map((p, i) => <Row key={i.toString()} player={p} game={game} />)
          }
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default PendingLobbyPlayersTable;
