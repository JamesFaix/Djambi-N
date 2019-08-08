import * as React from 'react';
import { LoginRequest, Game } from '../../api/model';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import * as Redirects from '../redirects';

interface LobbyPageProps {
    game : Game
}

interface LobbyPageState {

}

export default class LobbyPage extends React.Component<LobbyPageProps, LobbyPageState> {

    private getGameJson() {
        if (this.props.game) {
            return JSON.stringify(this.props.game);
        } else {
            return "(no game loaded)";
        }
    }

    render() {
        return (
            <div>
                <Redirects.ToHomeIfNoSession/>
                <Redirects.ToHomeIfNoGame/>
                {this.getGameJson()}
            </div>
        );
    }
}
