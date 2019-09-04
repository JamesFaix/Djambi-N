import * as React from 'react';
import { Classes } from '../../styles/styles';
import { State } from '../../store/root';
import { connect } from 'react-redux';

interface TitleSectionProps {
    title : string
}

const titleSection : React.SFC<TitleSectionProps> = props => {
    return (
        <div
            id={"title-section"}
            className={Classes.topBarTitle}
        >
            <h1>
                {props.title}
            </h1>
        </div>
    );
};

const mapStateToProps = (state : State) => {
    return {
        title: state.display.theme.copy.gameTitle
    };
}

const TitleSection = connect(mapStateToProps)(titleSection);
export default TitleSection;