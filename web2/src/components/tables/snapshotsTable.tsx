import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { SnapshotInfo } from '../../api/model';
import { SectionHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import SnapshotRow from './snapshotRow';

interface SnapshotsTableProps {
    snapshots : SnapshotInfo[]
}

const snapshotsTable : React.SFC<SnapshotsTableProps> = props => {
    return (<>
        <SectionHeader text="Game snapshots"/>
        <table className={Classes.stripedTable}>
            <tbody>
                <tr>
                    <th>ID</th>
                    <th>Description</th>
                    <th>Created on</th>
                    <th>Created by</th>
                    <th></th>
                    <th></th>
                </tr>
                {
                    props.snapshots
                        ? props.snapshots.map((s, i) =>
                            <SnapshotRow
                                key={i}
                                snapshot={s}
                            />)
                        : null
                }
            </tbody>
        </table>
    </>);
}

const mapStateToProps = (state : State) => {
    return {
        snapshots: state.activeGame.snapshots
    };
}

const SnapshotsTable = connect(mapStateToProps)(snapshotsTable);
export default SnapshotsTable;