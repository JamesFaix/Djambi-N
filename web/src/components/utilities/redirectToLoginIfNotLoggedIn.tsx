import * as React from 'react';
import { User } from "../../api/model";
import { State } from '../../store/root';
import { connect } from 'react-redux';
import Controller from '../../controllers/controller';

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
        user: state.session.user,
        restoreSessionOrRedirect: () => Controller.Session.redirectToLoginIfNotLoggedIn()
    };
};

const RedirectToLoginIfNotLoggedIn = connect(mapStateToProps)(redirectToLoginIfNotLoggedIn);
export default RedirectToLoginIfNotLoggedIn;