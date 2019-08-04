import * as React from 'react';
import { AppState } from "../store/state";

interface AppProps {
    appState : AppState
}

export default class App extends React.Component<AppProps>{
    render() {
        return <div>Hello app!</div>;
    }
}