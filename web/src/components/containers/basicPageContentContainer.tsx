import * as React from 'react';
import { Classes } from '../../styles/styles';

const BasicPageContentContainer : React.SFC<{}> = props => {
    return (
        <div
            id={"basic-page-content-container"}
            className={Classes.basicPageContentContainer}
        >
            {props.children}
        </div>
    );
}

export default BasicPageContentContainer;