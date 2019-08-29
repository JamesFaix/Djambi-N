import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconInfo } from '../../utilities/icons';

interface IconButtonProps {
    onClick: () => void,
    icon: IconInfo,
    showTitle ?: boolean
}

const IconButton : React.SFC<IconButtonProps> = props => {
    const style = props.showTitle ? {display:"flex"} : null;

    return (
        <button
            title={props.icon.title}
            onClick={props.onClick}
            style={style}
        >
            <FontAwesomeIcon icon={props.icon.icon}/>
            {props.showTitle ?
                <div style={{padding: "0px 0px 0px 5px"}}>
                    {props.icon.title}
                </div>
            : null}
        </button>
    );
}
export default IconButton;