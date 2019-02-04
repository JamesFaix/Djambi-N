import * as React from 'react';
import '../index.css';
import StyleService from '../styleService';

export default class AppTitle extends React.Component {

    render() {
        return (
            <div className={StyleService.classAppTitle}>
                <div className={StyleService.classAppTitleCharA}>D</div>
                <div className={StyleService.classAppTitleCharB}>J</div>
                <div className={StyleService.classAppTitleCharA}>A</div>
                <div className={StyleService.classAppTitleCharB}>M</div>
                <div className={StyleService.classAppTitleCharA}>B</div>
                <div className={StyleService.classAppTitleCharB}>I</div>
                <div className={StyleService.classAppTitleCharA}>-</div>
                <div className={StyleService.classAppTitleCharB}>N</div>
            </div>
        );
    }
}