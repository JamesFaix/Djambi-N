import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import Styles from '../../styles/styles';

interface IconButtonProps {
    title: string,
    onClick: () => void,
    icon: IconDefinition
}

const IconButton : React.SFC<IconButtonProps> = props => {
    return (
        <button
            title={props.title}
            onClick={props.onClick}
            style={Styles.iconButton()}
        >
            <FontAwesomeIcon
                icon={props.icon}
            />
        </button>
    );
};

export default IconButton;