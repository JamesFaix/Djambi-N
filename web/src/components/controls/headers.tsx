import * as React from 'react';
import { Classes } from '../../styles/styles';
import { IconInfo } from '../../utilities/icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

interface SectionHeaderProps {
    text : string,
}

export const SectionHeader : React.SFC<SectionHeaderProps> = props => {
    return (
        <div className={Classes.sectionHeader}>
            {props.text}
        </div>
    );
};

interface TimelineHeaderProps {
    icon : IconInfo
}

export const TimelineHeader : React.SFC<TimelineHeaderProps> = props => {
    const i = props.icon;
    return (
        <div
            className={Classes.timelineHeader}
            title={i.title}
        >
            <FontAwesomeIcon icon={i.icon}/>
            {` ${i.title}`}
        </div>
    );
};