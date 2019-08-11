import * as React from 'react';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import CreateGameForm from '../forms/createGameForm';

export default class CreateGamePage extends React.Component<{}> {

    render() {
        return (
            <div>
                <RedirectToLoginIfNotLoggedIn/>
                <Link to={Routes.dashboard}>
                    <button>
                        Home
                    </button>
                </Link>
                <CreateGameForm/>
            </div>
        );
    }
}