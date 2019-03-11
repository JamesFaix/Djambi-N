import * as React from 'react';
import { SnapshotInfo } from '../../../api/model';
import { Kernel as K } from '../../../kernel';
import ActionButtonCell from '../../tables/actionButtonCell';

export interface SnapshotsTableProps {
    snapshots : SnapshotInfo[],
    loadSnapshot : (snapshotId : number) => void,
    deleteSnapshot : (snapshotId : number) => void
}

export default class SnapshotsTable extends React.Component<SnapshotsTableProps> {

    render() {
        if (this.props.snapshots.length === 0) {
            return (
                <div className={K.classes.lightText}>
                    (No snapshots saved for this game.)
                </div>
            );
        }

        return (
            <div>
                <table className={K.classes.table}>
                    <tbody>
                        <tr>
                            <th>Snapshot #</th>
                            <th>Description</th>
                            <th>Created on</th>
                            <th>Created by</th>
                            <th></th>
                            <th></th>
                        </tr>
                        {
                            this.props.snapshots.map((s, i) =>
                                this.renderRow(s, i))
                        }
                    </tbody>
                </table>
            </div>
        );
    }

    private renderRow(snapshot : SnapshotInfo, rowNumber : number) {
        return (
            <tr key={"row" + rowNumber}>
                <td className={K.classes.rightAligned}>
                    {snapshot.id}
                </td>
                <td>
                    {snapshot.description}
                    </td>
                <td>
                    {K.dates.format(snapshot.createdOn)}
                </td>
                <td className={K.classes.rightAligned}>
                    {snapshot.createdByUserId}
                </td>
                <ActionButtonCell
                    label="Load"
                    onClick={() => this.props.loadSnapshot(snapshot.id)}
                />
                <ActionButtonCell
                    label="Delete"
                    onClick={() => this.props.deleteSnapshot(snapshot.id)}
                />
            </tr>
        );
    }
}