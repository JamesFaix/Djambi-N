import * as React from 'react';
import AppTitle from './appTitle';
import { Kernel as K } from '../kernel';

export default class TopMenu extends React.Component {

    public render() : JSX.Element {
        return (
            <div className={K.classes.topMenu}>
                <AppTitle/>
            </div>
        );
    }
}