import { Game } from "../api/model";
import * as React from 'react';
import LinkButton from "./linkButton";
import moment = require("moment");

export interface LobbiesTableProps {
    games : Game[]
}

export default class LobbiesTable extends React.Component<LobbiesTableProps> {

    renderLobbyRow(game : Game, rowNumber : number) {
        return (
            <tr key={"row" + rowNumber}>
                <td>
                    <LinkButton
                        label="Go"
                        to={"/lobby/" + game.id}
                    />
                </td>
                <td>
                    {moment(game.createdOn).format('MM/DD/YY hh:mm a')}
                </td>
                <td>{game.createdByUserId}</td>
                <td className="centeredContainer">
                    {game.parameters.regionCount}
                </td>
                <td className="centeredContainer">
                    <input
                        type="checkbox"
                        checked={game.parameters.isPublic}
                        disabled={true}
                    />
                </td>
                <td className="centeredContainer">
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
                <table className="table">
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
                        {this.props.games.map((lobby, i) => this.renderLobbyRow(lobby, i))}
                    </tbody>
                </table>
            </div>
        );
    }
}