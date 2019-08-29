import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

interface IconButtonProps {
    title: string,
    onClick: () => void,
    icon: IconDefinition,
    showTitle ?: boolean
}

const IconButton : React.SFC<IconButtonProps> = props => {
    const style = props.showTitle ? {display:"flex"} : null;

    return (
        <button
            title={props.title}
            onClick={props.onClick}
            style={style}
        >
            <FontAwesomeIcon icon={props.icon}/>
            {props.showTitle ?
                <div style={{padding: "0px 0px 0px 5px"}}>
                    {props.title}
                </div>
            : null}
        </button>
    );
}
export default IconButton;