import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { Classes } from '../../styles/styles';

interface IconButtonProps {
    title: string,
    onClick: () => void,
    icon: IconDefinition
}

interface IconButtonState {
    isMouseOver : boolean
}

class IconButton extends React.Component<IconButtonProps, IconButtonState> {
    constructor(props : IconButtonProps) {
        super(props);
        this.state = {
            isMouseOver: false
        };
    }

    render() {
        return (
            <button
                title={this.props.title}
                onClick={this.props.onClick}
                className={Classes.iconButton(this.state.isMouseOver)}
                onMouseOver={() => this.setState({isMouseOver: true})}
                onMouseOut={() => this.setState({isMouseOver: false})}
            >
                <FontAwesomeIcon
                    icon={this.props.icon}
                />
            </button>
        );
    }
};

export default IconButton;