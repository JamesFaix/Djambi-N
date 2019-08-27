import * as React from 'react';
import { User } from "../../api/model";
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import ApiActions from '../../apiActions';

interface RedirectToLoginIfNotLoggedInProps {
    user : User,
    restoreSessionOrRedirect: () => void
}

class redirectToLoginIfNotLoggedIn extends React.Component<RedirectToLoginIfNotLoggedInProps>{
    componentDidMount() {
        if (!this.props.user) {
            this.props.restoreSessionOrRedirect();
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
        restoreSessionOrRedirect: () => ApiActions.redirectToLoginIfNotLoggedIn()(dispatch)
    };
};

const RedirectToLoginIfNotLoggedIn = connect(mapStateToProps, mapDispatchToProps)(redirectToLoginIfNotLoggedIn);

export default RedirectToLoginIfNotLoggedIn;