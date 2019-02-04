import * as React from 'react';
import '../index.css';
import StyleService from '../styleService';

export interface PageTitleProps {
    label : string
}

export default class PageTitle extends React.Component<PageTitleProps> {

    render() {
        return (
            <div className={StyleService.classPageTitle}>
                {this.props.label}
            </div>
        );
    }
}