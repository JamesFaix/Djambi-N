import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyResponse, LobbiesQueryRequest } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import * as moment from 'moment';
import { Link } from 'react-router-dom';

export interface FindLobbyPageProps {
    user : UserResponse,
    api : ApiClient,
}

export interface FindLobbyPageState {
    lobbies : LobbyResponse[],
    query : LobbiesQueryRequest
}

export default class FindLobbyPage extends React.Component<FindLobbyPageProps, FindLobbyPageState> {
    constructor(props : FindLobbyPageProps) {
        super(props);
        this.state = {
            lobbies : [],
            query : {
                descriptionContains: null,
                createdByUserId: null,
                playerUserId: null,
                isPublic: null,
                allowGuests: null
            }
        };
    }

    componentDidMount() {
        this.refreshResults();
    }

    private refreshResults() {
        this.props.api
        .getLobbies(this.state.query)
        .then(lobbies => {
            this.setState({lobbies : lobbies});
        })
        .catch(reason => {
            alert("Get lobby failed because " + reason);
        });
    }

//---Rendering---

    renderLobbyRow(lobby : LobbyResponse, rowNumber : number) {
        return (
            <tr>
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

    renderLobbiesTable() {
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
                        {this.state.lobbies.map((lobby, i) => this.renderLobbyRow(lobby, i))}
                    </tbody>
                </table>
            </div>
        );
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"Find Game"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="My Games" to="/myGames"/>
                    <LinkButton label="Create Game" to="/createGame"/>
                </div>
                <br/>
                {this.renderLobbiesTable()}
            </div>
        );
    }
}