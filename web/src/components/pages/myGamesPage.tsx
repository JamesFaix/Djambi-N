import * as React from 'react';
import GamesQueryResultsTable from '../gamesQueryResultsTable';
import LinkButton from '../controls/linkButton';
import PageTitle from '../pageTitle';
import { Game, GamesQuery, User } from '../../api/model';
import { Kernel as K } from '../../kernel';
import { Redirect } from 'react-router';

export interface MyGamesPageProps {
    user : User
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
            createdByUserName: null,
            playerUserName: this.props.user.name, //My Games page only shows games where you are a player
            isPublic: null,
            allowGuests: null,
            descriptionContains: null,
            status: null
        }

        K.api
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
            return <Redirect to={K.routes.home()}/>
        }

        return (
            <div>
                <PageTitle label={"My Games"}/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <LinkButton label="Home" to={K.routes.dashboard()}/>
                    <LinkButton label="Create Game" to={K.routes.createGame()}/>
                    <LinkButton label="Find Game" to={K.routes.findGame()}/>
                </div>
                <br/>
                <GamesQueryResultsTable
                    games={this.state.games}
                />
            </div>
        );
    }
}