import * as React from 'react';
import { Kernel as K } from '../kernel';

export interface PageTitleProps {
    label : string
}

export default class PageTitle extends React.Component<PageTitleProps> {

    public render() : JSX.Element {
        return (
            <div className={K.classes.pageTitle}>
                {this.props.label}
            </div>
        );
    }
}