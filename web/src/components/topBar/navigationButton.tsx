import * as React from 'react';
import IconButton from '../controls/iconButton';
import { IconInfo } from '../../utilities/icons';
import Controller from '../../controller';

export enum ButtonState {
    Hidden = "HIDDEN",
    Active = "ACTIVE",
    Inactive = "INACTIVE"
}

const NavigationButton : React.SFC<{
    route : string,
    state : ButtonState,
    icon : IconInfo,
}> = props => {
    if (props.state === ButtonState.Hidden) {
        return null;
    }

    return (
        <IconButton
            icon={props.icon}
            onClick={() => Controller.navigateTo(props.route)}
            active={props.state === ButtonState.Active}
        />
    );
};
export default NavigationButton;