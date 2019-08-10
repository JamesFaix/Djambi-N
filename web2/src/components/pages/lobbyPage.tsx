import * as React from 'react';
import * as Redirects from '../redirects';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import GameParametersTable from '../lobby/gameParametersTable';
import LobbyPlayersTable from '../lobby/lobbyPlayersTable';

export default class LobbyPage extends React.Component<{}> {
    render() {
        return (
            <div>
                <Redirects.ToHomeIfNoSession/>
                <Redirects.ToHomeIfNoGame/>
                <Link to={Routes.dashboard}>
                    <button>
                        Home
                    </button>
                </Link>
                <GameParametersTable/>
                <LobbyPlayersTable/>
            </div>
        );
    }
}