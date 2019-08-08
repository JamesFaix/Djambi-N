import * as React from 'react';
import * as Redirects from '../redirects';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { connect } from 'react-redux';

interface DashboardPageProps {
    onLogoutClicked : () => void,
}

class dashboardPage extends React.Component<DashboardPageProps>{

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

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onLogoutClicked: () => ThunkActions.logout()(dispatch)
    };
}

const DashboardPage = connect(null, mapDispatchToProps)(dashboardPage);

export default DashboardPage;