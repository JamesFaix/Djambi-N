import * as React from 'react';

interface HeaderProps {
    text : string,
}

const style : React.CSSProperties = {textAlign:"center"};

export const SectionHeader : React.SFC<HeaderProps> = props => <h3 style={style}>{props.text}</h3>;
export const PlayHeader : React.SFC<HeaderProps> = props => <h4 style={style}>{props.text}</h4>;