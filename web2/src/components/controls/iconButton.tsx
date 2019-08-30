import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconInfo } from '../../utilities/icons';
import { Classes } from '../../styles/styles';

interface IconButtonProps {
    onClick: () => void,
    icon: IconInfo,
    showTitle ?: boolean
}

const IconButton : React.SFC<IconButtonProps> = props => {
    return (
        <button
            title={props.icon.title}
            onClick={props.onClick}
        >
            <FontAwesomeIcon icon={props.icon.icon}/>
            {props.showTitle ?
                <span className={Classes.iconBox}>
                    {props.icon.title}
                </span>
            : null}
        </button>
    );
}
export default IconButton;