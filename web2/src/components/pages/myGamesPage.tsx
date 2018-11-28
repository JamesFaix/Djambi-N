import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyResponse, LobbiesQueryRequest } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import LobbiesTable from '../lobbiesTable';

export interface MyGamesPageProps {
    user : UserResponse,
    api : ApiClient,
}

export interface MyGamesPageState {
    lobbies : LobbyResponse[],
}

export default class MyGamesPage extends React.Component<MyGamesPageProps, MyGamesPageState> {
    constructor(props : MyGamesPageProps) {
        super(props);
        this.state = {
            lobbies : []
        };
    }

    componentDidMount() {
        this.refreshResults();
    }

    private refreshResults() {
        const query : LobbiesQueryRequest = {
            createdByUserId: null,
            playerUserId: this.props.user.id,
            isPublic: null,
            allowGuests: null,
            descriptionContains: null
        }

        this.props.api
            .getLobbies(query)
            .then(lobbies => {
                this.setState({lobbies : lobbies});
            })
            .catch(reason => {
                alert("Get lobby failed because " + reason);
            });
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"My Games"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="Create Game" to="/createGame"/>
                    <LinkButton label="Find Game" to="/findGame"/>
                </div>
                <br/>
                <LobbiesTable
                    lobbies={this.state.lobbies}
                />
            </div>
        );
    }
}