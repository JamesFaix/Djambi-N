import * as React from 'react';
import { connect } from 'react-redux';
import { SnapshotInfo } from '../../api/model';
import { dateToString } from '../../utilities/dates';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import { State } from '../../store/root';
import Controller from '../../controller';

interface SnapshotRowProps {
    gameId : number,
    snapshot : SnapshotInfo,
    load : (gameId : number, snapshotId : number) => void,
    delete : (gameId : number, snapshotId : number) => void
}

const snapshotRow : React.SFC<SnapshotRowProps> = props => {
    if (!props.gameId) {
        return null;
    }

    const s = props.snapshot;
    return (
        <tr>
            <td>
                {s.id}
            </td>
            <td>
                {s.description}
            </td>
            <td>
                {dateToString(s.createdBy.time)}
            </td>
            <td>
                {s.createdBy.userName}
            </td>
            <td>
                <IconButton
                    icon={Icons.Snapshots.load}
                    onClick={() => props.load(props.gameId, s.id)}
                />
            </td>
            <td>
                <IconButton
                    icon={Icons.Snapshots.delete}
                    onClick={() => props.delete(props.gameId, s.id)}
                />
            </td>
        </tr>
    )
}

const mapStateToProps = (state : State) => {
    return {
        gameId: state.activeGame.game ? state.activeGame.game.id : null,
        load: (gameId : number, snapshotId : number) => Controller.Snapshots.load(gameId, snapshotId),
        delete: (gameId : number, snapshotId : number) => Controller.Snapshots.delete(gameId, snapshotId),
    };
}

const SnapshotRow = connect(mapStateToProps)(snapshotRow);
export default SnapshotRow;