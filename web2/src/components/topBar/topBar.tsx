import * as React from 'react';
import { AppState } from "../../store/state";
import TitleSection from './titleSection';
import UserSection from './userSection';

interface TopBarProps {
    appState : AppState
}

export default class TopBar extends React.Component<TopBarProps>{
    render() {
        const style = {
            height: "50px",
            borderStyle: "solid",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center"
        };

        const session = this.props.appState.session;
        return (
            <div style={style}>
                <TitleSection></TitleSection>
                <UserSection session={session}></UserSection>
            </div>
        );
    }
}