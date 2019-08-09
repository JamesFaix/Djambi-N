import * as React from 'react';
import { connect } from 'react-redux';
import { AppState } from '../store/state';
import { Dispatch } from 'redux';
import * as ThunkActions from '../thunkActions';
import { User } from '../api/model';

interface SessionRestorerProps {
    user : User,
    restore : () => void
}

class sessionRestorer extends React.Component<SessionRestorerProps> {
    componentDidMount() {
        if (!this.props.user) {
            this.props.restore();
        }
    }

    render() : JSX.Element {
        return null;
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        user: state.user
    };
};

const mapDispatchToProps = () => {
    return function (dispatch: Dispatch) {
        return {
            restore: () => ThunkActions.restoreSession()(dispatch)
        };
    };
};

const SessionRestorer = connect(mapStateToProps, mapDispatchToProps)(sessionRestorer);

export default SessionRestorer;