import * as React from 'react';
import { User } from '../../api/model';

export interface SnapshotsPageProps {
    user : User,
    gameId : number
}

export interface SnapshotsPageState {
}

export default class SnapshotsPage extends React.Component<SnapshotsPageProps, SnapshotsPageState> {
    constructor(props : SnapshotsPageProps) {
        super(props);
        this.state = {

        };
    }

    render() {
        return <div></div>;
    }
}