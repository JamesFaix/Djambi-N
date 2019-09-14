import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconInfo } from '../../utilities/icons';
import { Classes } from '../../styles/styles';

const IconButton : React.SFC<{
    onClick: () => void,
    icon: IconInfo,
    showTitle ?: boolean,
    active ?: boolean,
    disabled ?: boolean,
    style ?: React.CSSProperties
}> = props => (
    <button
        style={props.style}
        title={props.icon.title}
        onClick={props.onClick}
        data-active={props.active}
        disabled={props.disabled || props.active}
    >
        <FontAwesomeIcon icon={props.icon.icon}/>
        {props.showTitle ?
            <span className={Classes.iconBox}>
                {props.icon.title}
            </span>
        : null}
    </button>
)
export default IconButton;