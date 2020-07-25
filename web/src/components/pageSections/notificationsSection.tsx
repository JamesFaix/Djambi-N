import * as React from 'react';
import { useSelector } from 'react-redux';
import { State as AppState } from '../../store/root';
import Selectors from '../../selectors';
import { NotificationInfo } from '../../store/notifications';
import { Classes } from '../../styles/styles';
import BasicPageMargin from '../containers/basicPageMargin';
import BasicPageContentContainer from '../containers/basicPageContentContainer';
import ThemeService from '../../themes/themeService';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';

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

const Message : React.SFC<{ text: string }> = props => {
    return (
        <p style={{ lineHeight: "30px" }}>
            {props.text}
        </p>
    );
}

const NotificationRow : React.SFC<{ notification : NotificationInfo }> = props => {
    const theme = Selectors.theme();
    const n = props.notification;
    const messages = n.message.split("\n");

    return (
        <div style={{
            border: theme.colors.border,
            borderWidth: "thin",
            borderStyle: "solid",
            color: theme.colors.text,
            background: ThemeService.getNotificationBackground(theme, n.type),
            padding: "10px",
            display: "flex"
        }}
        >
            <div>
                {messages.map((m, i) => <Message text={m} key={i}/>)}
            </div>
            <IconButton
                icon={Icons.UserActions.dismiss}
                onClick={() => Controller.removeNotification(n.id)}
            />
        </div>
    );
}