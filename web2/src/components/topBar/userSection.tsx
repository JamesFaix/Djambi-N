import * as React from 'react';
import { Session } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';

interface UserSectionProps {
    session : Session,
    onLogoutClicked : () => void
}

const userSection : React.SFC<UserSectionProps> = props => {
    return props.session
        ? loggedInUserSection(props)
        : notLoggedInUserSection(props);
}

const notLoggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (
        <div style={{textAlign: "right"}}>
            (Not signed in)
        </div>
    );
};

const loggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (
        <div style={{textAlign: "right"}}>
            {props.session.user.name}
            <button
                onClick={_ => props.onLogoutClicked()}
            >
                Log out
            </button>
        </div>
    );
};

const mapStateToProps = (state : AppState) => {
    return {
        session: state.session
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onLogoutClicked: () => ThunkActions.logout()(dispatch)
    };
}

const UserSection = connect(mapStateToProps, mapDispatchToProps)(userSection);

export default UserSection;