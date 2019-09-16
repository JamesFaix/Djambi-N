import * as React from 'react';
import { useSelector } from 'react-redux';
import { State as AppState } from '../../store/root';
import BasicPageContentContainer from '../containers/basicPageContentContainer';
import BasicPageContainer from '../containers/basicPageContainer';
import Selectors from '../../selectors';

const NotificationsSection : React.SFC<{}> = _ => {
    const errors = useSelector((state : AppState) => [...state.errors.errors.values()]);

    if (errors.length === 0) {
        return null;
    }

    return (
        <BasicPageContainer>
            {errors.map((e, i) => <NotificationRow key={i} message={e}/>)}
        </BasicPageContainer>
    );
}
export default NotificationsSection;

const NotificationRow : React.SFC<{ message : string }> = props => {
    const theme = Selectors.theme();
    return (
        <div style={{
            border: theme.colors.border,
            borderWidth: "thin",
            borderStyle: "solid",
            color: "red",
            background: theme.colors.background,
            padding: "10px"
        }}
        >
            {props.message}
        </div>
    )
}