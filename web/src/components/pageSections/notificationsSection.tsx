import * as React from 'react';
import { useSelector } from 'react-redux';
import { State as AppState } from '../../store/root';
import Selectors from '../../selectors';
import { NotificationInfo, NotificationType } from '../../store/notifications';
import { Theme } from '../../themes/model';
import { Classes } from '../../styles/styles';
import BasicPageMargin from '../containers/basicPageMargin';
import BasicPageContentContainer from '../containers/basicPageContentContainer';

const NotificationsSection : React.SFC<{}> = _ => {
    const notifications = useSelector((state : AppState) => state.notifications.items);

    if (notifications.length === 0) {
        return null;
    }

    return (
        <div
            className={Classes.basicPageContainer}
            style={{
                position: "absolute",
                bottom: 0,
                width: "100%"
            }}
        >
            <BasicPageMargin/>
            <BasicPageContentContainer>
                {notifications.map((n, i) =>
                    <NotificationRow key={i} notification={n}/>)
                }
            </BasicPageContentContainer>
            <BasicPageMargin/>
        </div>
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
            return "palegoldenrod";
        default:
            return theme.colors.background;
    }
}