import * as React from 'react';
import { User } from "../../api/model";
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import SessionStoreFlows from '../../storeFlows/session';

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

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        restoreSessionAndRedirect: () => SessionStoreFlows.redirectToDashboardIfLoggedIn()(dispatch)
    };
};

const RedirectToDashboardIfLoggedIn = connect(mapStateToProps, mapDispatchToProps)(redirectToDashboardIfLoggedIn);

export default RedirectToDashboardIfLoggedIn