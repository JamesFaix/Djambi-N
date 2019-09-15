import * as React from 'react';
import { Classes } from '../../styles/styles';

const PlayPageContainer : React.SFC<{}> = props => {
    return (
        <div
            id={"play-page-container"}
            className={Classes.playPageContainer}
        >
            {props.children}
        </div>
    )
}

export default PlayPageContainer;