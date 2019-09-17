import * as React from 'react';
import { useSelector } from 'react-redux';
import { State as AppState } from '../../store/root';
import BasicPageContainer from '../containers/basicPageContainer';
import Selectors from '../../selectors';
import { NotificationInfo, NotificationType } from '../../store/notifications';
import { Theme } from '../../themes/model';

const NotificationsSection : React.SFC<{}> = _ => {
    const notifications = useSelector((state : AppState) => state.notifications.items);

    if (notifications.length === 0) {
        return null;
    }

    return (
        <BasicPageContainer>
            {notifications.map((n, i) =>
                <NotificationRow key={i} notification={n}/>)
            }
        </BasicPageContainer>
    );
}
export default NotificationsSection;

const NotificationRow : React.SFC<{ notification : NotificationInfo }> = props => {
    const theme = Selectors.theme();
    const n = props.notification;
    return (
        <div style={{
            border: theme.colors.border,
            borderWidth: "thin",
            borderStyle: "solid",
            color: theme.colors.text,
            background: getBackgroundColor(n, theme),
            padding: "10px",
            opacity: 0.75
        }}
        >
            {n.message}
        </div>
    )
}

function getBackgroundColor(notification : NotificationInfo, theme : Theme) : string {
    switch (notification.type) {
        case NotificationType.Error:
            return "lightcoral";
        case NotificationType.Info:
        default:
            return theme.colors.background;
    }
}