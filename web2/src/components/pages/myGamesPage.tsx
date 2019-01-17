import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User, Game, GamesQuery } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import LobbiesTable from '../lobbiesTable';

export interface MyGamesPageProps {
    user : User,
    api : ApiClient,
}

export interface MyGamesPageState {
    games : Game[],
}

export default class MyGamesPage extends React.Component<MyGamesPageProps, MyGamesPageState> {
    constructor(props : MyGamesPageProps) {
        super(props);
        this.state = {
            games : []
        };
    }

    componentDidMount() {
        this.refreshResults();
    }

    private refreshResults() {
        const query : GamesQuery = {
            gameId: null,
            createdByUserId: null,
            playerUserId: this.props.user.id,
            isPublic: null,
            allowGuests: null,
            descriptionContains: null
        }

        this.props.api
            .getGames(query)
            .then(games => {
                this.setState({games : games});
            })
            .catch(reason => {
                alert("Get games failed because " + reason);
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
                    games={this.state.games}
                />
            </div>
        );
    }
}