import * as React from 'react';

interface SectionHeaderProps {
    text : string
}

const SectionHeader : React.SFC<SectionHeaderProps> = props => {
    return (
        <h3 style={{textAlign:"center"}}>
            {props.text}
        </h3>
    );
}

export default SectionHeader;