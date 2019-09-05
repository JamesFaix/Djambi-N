import * as React from 'react';
import { User } from "../../api/model";
import { State } from '../../store/root';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import SessionStoreFlows from '../../storeFlows/session';

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

const mapStateToProps = (state: State) => {
    return {
        user: state.session.user
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        restoreSessionOrRedirect: () => SessionStoreFlows.redirectToLoginIfNotLoggedIn()(dispatch)
    };
};

const RedirectToLoginIfNotLoggedIn = connect(mapStateToProps, mapDispatchToProps)(redirectToLoginIfNotLoggedIn);

export default RedirectToLoginIfNotLoggedIn;