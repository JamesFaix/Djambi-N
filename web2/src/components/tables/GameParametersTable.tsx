import React, { FC } from 'react';
import {
  Table, TableContainer, TableCell, TableRow, TableBody,
} from '@material-ui/core';
import { useSelector } from 'react-redux';
import { selectActiveGame } from '../../hooks/selectors';

const toYesNo = (value: boolean): string => {
  return value ? 'Yes' : 'No';
};

const GameParametersTable: FC = () => {
  const { game } = useSelector(selectActiveGame);

  if (game === null) {
    return <></>;
  }

  return (
    <TableContainer>
      <Table>
        <colgroup>
          <col width="50%" />
          <col width="50%" />
        </colgroup>
        <TableBody>
          <TableRow>
            <TableCell>Description</TableCell>
            <TableCell>{game.parameters.description}</TableCell>
          </TableRow>
          <TableRow>
            <TableCell>Allow guests</TableCell>
            <TableCell>{toYesNo(game.parameters.allowGuests)}</TableCell>
          </TableRow>
          <TableRow>
            <TableCell>Public</TableCell>
            <TableCell>{toYesNo(game.parameters.isPublic)}</TableCell>
          </TableRow>
          <TableRow>
            <TableCell>Board regions</TableCell>
            <TableCell>{game.parameters.regionCount}</TableCell>
          </TableRow>
        </TableBody>
      </Table>
    </TableContainer>
  );
};

export default GameParametersTable;
