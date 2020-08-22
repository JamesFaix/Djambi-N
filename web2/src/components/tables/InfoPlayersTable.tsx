import React, { FC } from 'react';
import {
  Table, TableContainer, TableHead, TableCell, TableRow, TableBody,
} from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectActiveGame } from '../../hooks/selectors';
import { PlayerKind, PlayerDto, GameDto } from '../../api-client';

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

const InfoPlayersTable: FC = () => {
  const { game } = useSelector(selectActiveGame);
  if (game === null) {
    return <></>;
  }

  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Note</TableCell>
            <TableCell>Status</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {
            game.players.map((p, i) => (
              <TableRow key={i.toString()}>
                <TableCell>{p.name}</TableCell>
                <TableCell>{getPlayerNote(p, game)}</TableCell>
                <TableCell>{p.status}</TableCell>
              </TableRow>
            ))
          }
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default InfoPlayersTable;
