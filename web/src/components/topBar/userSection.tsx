import * as React from 'react';
import { User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import { Classes } from '../../styles/styles';
import Controller from '../../storeFlows/controller';

interface UserSectionProps {
    user : User,
    onLogoutClicked : () => void
}

const userSection : React.SFC<UserSectionProps> = props => {
    return (
        <div
            id={"user-section"}
            className={Classes.topBarUser}
        >
            {
                props.user
                    ? loggedInUserSection(props)
                    : notLoggedInUserSection(props)
            }
        </div>
    );
}

const notLoggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (<>
        (Not signed in)
    </>);
};

const loggedInUserSection : React.SFC<UserSectionProps> = props => {
    return (<>
        {props.user.name}
        <IconButton
            icon={Icons.UserActions.logout}
            onClick={() => props.onLogoutClicked()}
        />
    </>);
};

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user
    };
};

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        onLogoutClicked: () => Controller.Session.logout()
    };
}

const UserSection = connect(mapStateToProps, mapDispatchToProps)(userSection);

export default UserSection;