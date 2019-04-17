import * as React from 'react';
import { Kernel as K } from '../kernel';

export default class AppTitle extends React.Component {

    public render() : JSX.Element {
        return (
            <div className={K.classes.appTitle}>
                <div className={K.classes.appTitleCharA}>D</div>
                <div className={K.classes.appTitleCharB}>J</div>
                <div className={K.classes.appTitleCharA}>A</div>
                <div className={K.classes.appTitleCharB}>M</div>
                <div className={K.classes.appTitleCharA}>B</div>
                <div className={K.classes.appTitleCharB}>I</div>
                <div className={K.classes.appTitleCharA}>-</div>
                <div className={K.classes.appTitleCharB}>N</div>
            </div>
        );
    }
}