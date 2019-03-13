import * as React from 'react';
import { Game } from '../api/model';
import { Kernel as K } from '../kernel';
import Button, { ButtonKind } from './controls/button';
import { IconKind } from './icon';

export interface GamesQueryResultsTableProps {
    games : Game[]
}

export default class GamesQueryResultsTable extends React.Component<GamesQueryResultsTableProps> {

    render() {
        return (
            <div>
                <table className={K.classes.table}>
                    <tbody>
                        <tr>
                            <th></th>
                            <th>Game #</th>
                            <th>Created on</th>
                            <th>Created by</th>
                            <th>Regions</th>
                            <th>Public</th>
                            <th>Guests allowed</th>
                            <th>Description</th>
                        </tr>
                        {this.props.games.map((game, i) => this.renderGameRow(game, i))}
                    </tbody>
                </table>
            </div>
        );
    }

    renderGameRow(game : Game, rowNumber : number) {
        return (
            <tr key={"row" + rowNumber}>
                <td>
                    <Button
                        kind={ButtonKind.Link}
                        icon={IconKind.Enter}
                        to={K.routes.gameInfo(game.id)}
                    />
                </td>
                <td className={K.classes.rightAligned}>
                    {game.id}
                </td>
                <td>
                    {K.dates.format(game.createdOn)}
                </td>
                <td>{game.createdByUserId}</td>
                <td className={K.classes.centerAligned}>
                    {game.parameters.regionCount}
                </td>
                <td className={K.classes.centerAligned}>
                    <input
                        type="checkbox"
                        checked={game.parameters.isPublic}
                        disabled={true}
                    />
                </td>
                <td className={K.classes.centerAligned}>
                    <input
                        type="checkbox"
                        checked={game.parameters.allowGuests}
                        disabled={true}
                    />
                </td>
                <td>{game.parameters.description}</td>
            </tr>
        );
    }
}