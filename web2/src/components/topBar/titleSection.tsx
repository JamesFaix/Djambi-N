import * as React from 'react';
import { Classes } from '../../styles/styles';

const TitleSection : React.SFC<{}> = _ => {
    return (
        <div
            id={"title-section"}
            className={Classes.topBarTitle}
        >
            <h1>
                Djambi-N
            </h1>
        </div>
    );
};

export default TitleSection;