import * as React from 'react';
import { User } from "../../api/model";
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import Controller from '../../storeFlows/controller';

interface RedirectToDashboardIfLoggedInProps {
    user : User,
    restoreSessionAndRedirect: () => void
}

class redirectToDashboardIfLoggedIn extends React.Component<RedirectToDashboardIfLoggedInProps>{
    componentDidMount() {
        if (!this.props.user) {
            this.props.restoreSessionAndRedirect();
        }
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (state: State) => {
    return {
        user: state.session.user
    };
};

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        restoreSessionAndRedirect: () => Controller.Session.redirectToDashboardIfLoggedIn()
    };
};

const RedirectToDashboardIfLoggedIn = connect(mapStateToProps, mapDispatchToProps)(redirectToDashboardIfLoggedIn);

export default RedirectToDashboardIfLoggedIn