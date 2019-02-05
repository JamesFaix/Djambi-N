import { Game } from "../api/model";
import * as React from 'react';
import LinkButton from "./controls/linkButton";
import Routes from "../routes";
import DateService from "../dateService";
import { Classes } from "../styles";

export interface GamesQueryResultsTableProps {
    games : Game[]
}

export default class GamesQueryResultsTable extends React.Component<GamesQueryResultsTableProps> {

    renderGameRow(game : Game, rowNumber : number) {
        return (
            <tr key={"row" + rowNumber}>
                <td>
                    <LinkButton label="Go" to={Routes.gameInfo(game.id)} />
                </td>
                <td>
                    {DateService.format(game.createdOn)}
                </td>
                <td>{game.createdByUserId}</td>
                <td className={Classes.centerAligned}>
                    {game.parameters.regionCount}
                </td>
                <td className={Classes.centerAligned}>
                    <input
                        type="checkbox"
                        checked={game.parameters.isPublic}
                        disabled={true}
                    />
                </td>
                <td className={Classes.centerAligned}>
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

    render() {
        return (
            <div>
                <table className={Classes.table}>
                    <tbody>
                        <tr>
                            <th></th>
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
}