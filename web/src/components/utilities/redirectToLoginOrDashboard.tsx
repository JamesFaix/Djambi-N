import * as React from 'react';
import { User } from "../../api/model";
import { State } from '../../store/root';
import { connect } from 'react-redux';
import Routes from '../../routes';
import Controller from '../../controllers/controller';

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

const mapStateToProps = (state: State) => {
    return {
        user: state.session.user,
        redirectToDashboard: () => Controller.navigateTo(Routes.dashboard),
        restoreSessionAndRedirect: () => Controller.Session.redirectToLoginOrDashboard()
    };
};

const RedirectToLoginOrDashboard = connect(mapStateToProps)(redirectToLoginOrDashboard);
export default RedirectToLoginOrDashboard