import * as React from 'react';
import * as Redirects from '../redirects';

interface DashboardPageProps {
    onLogoutClicked : () => void,
}

export default class DashboardPage extends React.Component<DashboardPageProps>{

    render() {
        return (
            <div>
                <Redirects.ToHomeIfNoSession/>
                Dashboard page content
                <button
                    onClick={() => this.props.onLogoutClicked()}
                >
                    Log out
                </button>
            </div>
        );
    }
}