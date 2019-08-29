import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { Classes } from '../../styles/styles';

interface IconBoxProps {
    icon : IconDefinition,
    title : string,
    color ?: string
}

const IconBox : React.SFC<IconBoxProps> = props => {
    return (
        <div
            className={Classes.iconBox}
            title={props.title}
        >
            <FontAwesomeIcon
                icon={props.icon}
                style={{color: props.color}}
            />
        </div>
    );
}

export default IconBox;