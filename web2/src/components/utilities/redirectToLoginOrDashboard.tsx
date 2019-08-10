import * as React from 'react';
import { User } from "../../api/model";
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';
import { navigateTo } from '../../history';
import Routes from '../../routes';

interface RedirectToLoginOrDashboardProps {
    user : User,
    restoreSessionAndRedirect: () => void
    redirectToDashboard: () => void
}

class redirectToLoginOrDashboard extends React.Component<RedirectToLoginOrDashboardProps>{
    componentDidMount() {
        if (this.props.user) {
            this.props.redirectToDashboard();
        } else {
            this.props.restoreSessionAndRedirect();
        }
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (state: AppState) => {
    return {
        user: state.session.user
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        redirectToDashboard: () => navigateTo(Routes.dashboard),
        restoreSessionAndRedirect: () => ThunkActions.redirectToLoginOrDashboard()(dispatch)
    };
};

const RedirectToLoginOrDashboard = connect(mapStateToProps, mapDispatchToProps)(redirectToLoginOrDashboard);

export default RedirectToLoginOrDashboard