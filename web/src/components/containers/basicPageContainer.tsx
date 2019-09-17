import * as React from 'react';
import { Classes } from '../../styles/styles';
import BasicPageMargin from './basicPageMargin';
import BasicPageContentContainer from './basicPageContentContainer';

const BasicPageContainer : React.SFC<{}> = props => {
    return (
        <div
            id={"basic-page-container"}
            className={Classes.basicPageContainer}
        >
            <BasicPageMargin/>
            <BasicPageContentContainer>
                {props.children}
            </BasicPageContentContainer>
            <BasicPageMargin/>
        </div>
    )
}

export default BasicPageContainer;