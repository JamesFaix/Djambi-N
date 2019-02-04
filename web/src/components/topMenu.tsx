import * as React from 'react';
import '../index.css';
import AppTitle from './appTitle';
import StyleService from '../styleService';

export default class TopMenu extends React.Component {

    render() {
        return (
            <div className={StyleService.classTopMenu}>
                <AppTitle/>
            </div>
        );
    }
}