import * as React from 'react';
import AppTitle from './appTitle';
import { Kernel as K } from '../kernel';

export default class TopMenu extends React.Component {

    render() {
        return (
            <div className={K.classes.topMenu}>
                <AppTitle/>
            </div>
        );
    }
}