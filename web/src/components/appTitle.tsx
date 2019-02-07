import * as React from 'react';
import { Classes } from '../styles';

export default class AppTitle extends React.Component {

    render() {
        return (
            <div className={Classes.appTitle}>
                <div className={Classes.appTitleCharA}>D</div>
                <div className={Classes.appTitleCharB}>J</div>
                <div className={Classes.appTitleCharA}>A</div>
                <div className={Classes.appTitleCharB}>M</div>
                <div className={Classes.appTitleCharA}>B</div>
                <div className={Classes.appTitleCharB}>I</div>
                <div className={Classes.appTitleCharA}>-</div>
                <div className={Classes.appTitleCharB}>N</div>
            </div>
        );
    }
}