import * as React from 'react';
import AppTitle from './appTitle';
import { Classes } from '../styles';

export default class TopMenu extends React.Component {

    render() {
        return (
            <div className={Classes.topMenu}>
                <AppTitle/>
            </div>
        );
    }
}