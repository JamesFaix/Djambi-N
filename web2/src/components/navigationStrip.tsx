import React from 'react';
import LinkButton, { LinkButtonProps } from './linkButton';
import '../index.css';

export interface NavigationStripProps {
    links : LinkButtonProps[]
}

export default class NavigationStrip extends React.Component<NavigationStripProps> {

    render() {
        return (
            <div className="navigationStrip">
                {this.props.links.map((link, index) =>
                    <LinkButton
                        key={index}
                        to={link.to}
                        label={link.label}
                    />
                )}
            </div>
        );
    }
}