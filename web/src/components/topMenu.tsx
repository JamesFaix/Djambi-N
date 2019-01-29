import * as React from 'react';
import '../index.css';
import AppTitle from './appTitle';

export default class TopMenu extends React.Component {

    render() {
        return (
            <div className="topMenu">
                <AppTitle/>
            </div>
        );
    }
}