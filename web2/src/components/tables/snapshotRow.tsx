import * as React from 'react';
import { connect } from 'react-redux';
import { SnapshotInfo } from '../../api/model';
import { dateToString } from '../../utilities/dates';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import { Dispatch } from 'redux';
import { State } from '../../store/root';
import SnapshotStoreFlows from '../../storeFlows/snapshots';

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
        gameId: state.activeGame.game ? state.activeGame.game.id : null
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        load: (gameId : number, snapshotId : number) => SnapshotStoreFlows.loadSnapshot(gameId, snapshotId)(dispatch),
        delete: (gameId : number, snapshotId : number) => SnapshotStoreFlows.deleteSnapshot(gameId, snapshotId)(dispatch),
    };
}

const SnapshotRow = connect(mapStateToProps, mapDispatchToProps)(snapshotRow);
export default SnapshotRow;