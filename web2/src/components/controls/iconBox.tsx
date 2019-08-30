import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Classes } from '../../styles/styles';
import { IconInfo } from '../../utilities/icons';

interface IconBoxProps {
    icon : IconInfo,
    color ?: string
}

const IconBox : React.SFC<IconBoxProps> = props => {
    return (
        <div
            className={Classes.iconBox}
            title={props.icon.title}
        >
            <FontAwesomeIcon
                icon={props.icon.icon}
                style={{color: props.color}}
            />
        </div>
    );
}

export default IconBox;