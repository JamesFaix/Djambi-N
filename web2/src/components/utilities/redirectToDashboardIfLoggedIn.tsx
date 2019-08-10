import * as React from 'react';
import { User } from "../../api/model";
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';

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

const mapStateToProps = (state: AppState) => {
    return {
        user: state.session.user
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        restoreSessionAndRedirect: () => ThunkActions.redirectToDashboardIfLoggedIn()(dispatch)
    };
};

const RedirectToDashboardIfLoggedIn = connect(mapStateToProps, mapDispatchToProps)(redirectToDashboardIfLoggedIn);

export default RedirectToDashboardIfLoggedIn