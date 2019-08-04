import * as React from 'react';
import { AppState } from '../../store/state';
import { Repository } from '../../repository';

interface DashboardPageProps {
    appState : AppState,
    repo: Repository
}

export default class DashboardPage extends React.Component<DashboardPageProps>{

    render() {
        return <div>Dashboard page content</div>;
    }
}