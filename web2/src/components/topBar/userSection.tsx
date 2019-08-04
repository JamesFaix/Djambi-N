import * as React from 'react';
import { Session } from '../../api/model';

interface UserSectionProps {
    session : Session
}

export default class UserSection extends React.Component<UserSectionProps>{
    render() {
        const style : React.CSSProperties = {
            textAlign: "right"
        };

        const userName = this.props.session
            ? this.props.session.user.name
            : "(Not logged in)";

        return (
            <div style={style}>
                {userName}
            </div>
        );
    }
}