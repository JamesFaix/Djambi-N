import { LobbyResponse } from "../api/model";
import * as React from 'react';
import LinkButton from "./linkButton";
import moment = require("moment");

export interface LobbiesTableProps {
    lobbies : LobbyResponse[]
}

export default class LobbiesTable extends React.Component<LobbiesTableProps> {

    renderLobbyRow(lobby : LobbyResponse, rowNumber : number) {
        return (
            <tr key={"row" + rowNumber}>
                <td>
                    <LinkButton
                        label="Go"
                        to={"/lobby/" + lobby.id}
                    />
                </td>
                <td>
                    {moment(lobby.createdOn).format('MM/DD/YY hh:mm a')}
                </td>
                <td>{lobby.createdByUserId}</td>
                <td className="centeredContainer">
                    {lobby.regionCount}
                </td>
                <td className="centeredContainer">
                    <input
                        type="checkbox"
                        checked={lobby.isPublic}
                        disabled={true}
                    />
                </td>
                <td className="centeredContainer">
                    <input
                        type="checkbox"
                        checked={lobby.allowGuests}
                        disabled={true}
                    />
                </td>
                <td>{lobby.description}</td>
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
                        {this.props.lobbies.map((lobby, i) => this.renderLobbyRow(lobby, i))}
                    </tbody>
                </table>
            </div>
        );
    }
}