import * as React from 'react';
import { Classes } from '../../styles/styles';

export const VerticalSpacerLarge : React.SFC<{}> = _ => {
    return <div className={Classes.verticalSpacerLarge}></div>;
}

export const VerticalSpacerSmall : React.SFC<{}> = _ => {
    return <div className={Classes.verticalSpacerSmall}></div>;
}