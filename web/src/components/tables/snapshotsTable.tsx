import * as React from 'react';
import { SnapshotInfo } from '../../api/model';
import { Kernel as K } from '../../kernel';
import Button, { ButtonKind } from '../controls/button';
import { IconKind } from '../icons/icon';

export interface SnapshotsTableProps {
    snapshots : SnapshotInfo[],
    loadSnapshot : (snapshotId : number) => void,
    deleteSnapshot : (snapshotId : number) => void
}

export default class SnapshotsTable extends React.Component<SnapshotsTableProps> {

    public render() : JSX.Element {
        if (this.props.snapshots.length === 0) {
            return (
                <div className={K.classes.combine([K.classes.table, K.classes.lightText])}>
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
                    {K.dates.format(snapshot.createdBy.time)}
                </td>
                <td className={K.classes.rightAligned}>
                    {snapshot.createdBy.userName}
                </td>
                <td className={K.classes.centerAligned}>
                    <Button
                        kind={ButtonKind.Action}
                        icon={IconKind.Load}
                        onClick={() => this.props.loadSnapshot(snapshot.id)}
                        hint="Load snapshot"
                    />
                </td>
                <td className={K.classes.centerAligned}>
                    <Button
                        kind={ButtonKind.Action}
                        icon={IconKind.Delete}
                        onClick={() => this.props.deleteSnapshot(snapshot.id)}
                        hint="Delete snapshot"
                    />
                </td>
            </tr>
        );
    }
}