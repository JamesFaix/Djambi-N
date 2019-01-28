import * as React from 'react';
import ApiClient from '../../api/client';
import { User, Game } from '../../api/model';
import LinkButton from '../linkButton';
import PageTitle from '../pageTitle';
import Routes from '../../routes';

export interface GamePageProps {
    user : User,
    api : ApiClient,
    gameId : number
}

export interface GamePageState {
    game : Game
}

export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    constructor(props : GamePageProps) {
        super(props);
        this.state = {
            game : null
        };
    }

    componentDidMount() {
        this.props.api
            .getGame(this.props.gameId)
            .then(game => {
                this.setState({game : game});
                return this.props.api
                    .getBoard(game.parameters.regionCount)
                    .then(board => {
                        console.log(board);
                    })
                    .catch(reason => {
                        alert("Get board failed because " + reason);
                    });
            })
            .catch(reason => {
                alert("Get game failed because " + reason);
            });
    }

    render() {
        return (
            <div>
                <PageTitle label={"Game"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to={Routes.dashboard()}/>
                    <LinkButton label="Rules" to={Routes.rules()} newWindow={true}/>
                </div>
                <br/>
                [Board]
            </div>
        );
    }
}