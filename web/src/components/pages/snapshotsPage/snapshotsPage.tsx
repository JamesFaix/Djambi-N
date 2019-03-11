import * as React from 'react';
import { User, SnapshotInfo, CreateSnapshotRequest } from '../../../api/model';
import SnapshotsTable from './snapshotsTable';
import { Kernel as K } from '../../../kernel';
import { Redirect } from 'react-router';
import PageTitle from '../../pageTitle';
import LinkButton from '../../controls/linkButton';
import LabeledInput from '../../controls/labeledInput';
import { InputTypes } from '../../../constants';
import ActionButton from '../../controls/actionButton';

export interface SnapshotsPageProps {
    user : User,
    gameId : number
}

export interface SnapshotsPageState {
    snapshots : SnapshotInfo[],
    redirectUrl : string,
    newSnapshotDescription : string
}

export default class SnapshotsPage extends React.Component<SnapshotsPageProps, SnapshotsPageState> {
    constructor(props : SnapshotsPageProps) {
        super(props);
        this.state = {
            snapshots: [],
            redirectUrl: null,
            newSnapshotDescription: ""
        };
    }

    componentDidMount() {
        this.getSnapshots();
    }

    render() {
        if (this.state.redirectUrl !== null) {
            return <Redirect to={this.state.redirectUrl}/>;
        }

        const title = "Snapshots for Game " + this.props.gameId;

        return (
            <div>
                <PageTitle label={title}/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <LinkButton label="Home" to={K.routes.dashboard()}/>
                    <LinkButton label="Back to Game" to={K.routes.game(this.props.gameId)}/>
                    <LinkButton label="My Games" to={K.routes.myGames()}/>
                    <LinkButton label="Find Game" to={K.routes.findGame()}/>
                </div>
                <br/>
                <SnapshotsTable
                    snapshots={this.state.snapshots}
                    loadSnapshot={id => this.loadSnapshot(id)}
                    deleteSnapshot={id => this.deleteSnapshot(id)}
                />
                <br/>
                <div className={K.classes.form}>
                    <LabeledInput
                        label="Description"
                        type={InputTypes.Text}
                        onChange={e => this.descriptionChanged(e)}
                    />
                </div>
                <br/>
                <div className={K.classes.centerAligned}>
                    <ActionButton
                        label="Create"
                        onClick={() => this.createSnapshot()}
                    />
                </div>
            </div>
        );
    }

    //--- State updates ---

    private descriptionChanged(e : React.ChangeEvent<HTMLInputElement>) {
        const description = e.target.value;
        this.setState({newSnapshotDescription: description})
    }

    private getSnapshots() {
        K.api.getSnapshotsForGame(this.props.gameId)
        .then(snapshots => {
            this.setState({snapshots: snapshots});
        })
    }

    private createSnapshot() : void {
        const request : CreateSnapshotRequest =
            {
                description: this.state.newSnapshotDescription
            };

        K.api.createSnapshot(this.props.gameId, request)
        .then(snapshot => {
            const newSnapshots = this.state.snapshots.slice();
            newSnapshots.push(snapshot);
            this.setState({
                snapshots: newSnapshots,
                newSnapshotDescription: ""
            });
        })
    }

    private loadSnapshot(snapshotId : number) : void {
        K.api.loadSnapshot(this.props.gameId, snapshotId)
        .then(_ => {
            const url = K.routes.game(this.props.gameId);
            this.setState({redirectUrl: url});
        });
    }

    private deleteSnapshot(snapshotId : number) : void {
        K.api.deleteSnapshot(this.props.gameId, snapshotId)
        .then(_ => {
            const newSnapshots = this.state.snapshots.filter(s => s.id !== snapshotId);
            this.setState({snapshots: newSnapshots});
        });
    }
}