import * as React from 'react';
import { Classes } from '../styles';

export interface PageTitleProps {
    label : string
}

export default class PageTitle extends React.Component<PageTitleProps> {

    render() {
        return (
            <div className={Classes.pageTitle}>
                {this.props.label}
            </div>
        );
    }
}