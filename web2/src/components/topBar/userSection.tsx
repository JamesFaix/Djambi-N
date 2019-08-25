import * as React from 'react';
import { User } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import IconButton from '../controls/iconButton';

interface UserSectionProps {
    user : User,
    onLogoutClicked : () => void
}

const userSection : React.SFC<UserSectionProps> = props => {
    return props.user
        ? loggedInUserSection(props)
        : notLoggedInUserSection(props);
}

const notLoggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (
        <div>
            (Not signed in)
        </div>
    );
};

const loggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (
        <div>
            {props.user.name}
            <IconButton
                title="Log out"
                icon={faSignOutAlt}
                onClick={() => props.onLogoutClicked()}
            />
        </div>
    );
};

const mapStateToProps = (state : AppState) => {
    return {
        user: state.session.user
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onLogoutClicked: () => ThunkActions.logout()(dispatch)
    };
}

const UserSection = connect(mapStateToProps, mapDispatchToProps)(userSection);

export default UserSection;