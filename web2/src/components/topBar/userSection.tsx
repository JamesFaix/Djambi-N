import * as React from 'react';
import { Session } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';

interface UserSectionProps {
    session : Session
}

const userSection : React.SFC<UserSectionProps> = props => {
    const style : React.CSSProperties = {
        textAlign: "right"
    };

    const userName = props.session
        ? props.session.user.name
        : "(Not logged in)";

    return (
        <div style={style}>
            {userName}
        </div>
    );
}

const mapStateToProps = (state : AppState) : UserSectionProps => {
    return {
        session: state.session
    };
};

const UserSection = connect(mapStateToProps)(userSection);

export default UserSection;