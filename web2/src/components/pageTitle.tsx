import React from 'react';
import '../index.css';

export interface PageTitleProps {
    label : string
}

export default class PageTitle extends React.Component<PageTitleProps> {

    render() {
        return (
            <div className="pageTitle">
                {this.props.label}
            </div>
        );
    }
}