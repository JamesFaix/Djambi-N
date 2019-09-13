import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../containers/basicPageContainer';
import LoadSnapshots from '../utilities/loadSnapshots';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import { Classes } from '../../styles/styles';
import { SnapshotInfo } from '../../api/model';
import { dateToString } from '../../utilities/dates';

export default class SnapshotsPage extends React.Component<{}>{
    render() {
        const gameId = (this.props as any).match.params.gameId;
        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <LoadSnapshots gameId={gameId}/>
                <SectionHeader text="Game snapshots"/>
                <SnapshotsTable/>
                <br/>
                <br/>
                <CreateSnapshotForm/>
            </BasicPageContainer>
        );
    }
}

const SnapshotsTable : React.SFC<{}> = _ => {
    const snapshots = useSelector((state : AppState) => state.activeGame.snapshots);
    return (
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
                {snapshots
                    ? snapshots.map((s, i) =>
                        <SnapshotRow
                            key={i}
                            snapshot={s}
                        />)
                    : null
                }
            </tbody>
        </table>
    );
}

const SnapshotRow : React.SFC<{
    snapshot : SnapshotInfo
}> = props => {
    const game = Selectors.game();
    if (!game) {
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
                    onClick={() => Controller.Snapshots.load(game.id, s.id)}
                />
            </td>
            <td>
                <IconButton
                    icon={Icons.Snapshots.delete}
                    onClick={() => Controller.Snapshots.delete(game.id, s.id)}
                />
            </td>
        </tr>
    )
}

const CreateSnapshotForm : React.SFC<{}> = _ => {
    const [description, setDescription] = React.useState("");
    const game = Selectors.game();
    const gameId = game ? game.id : null;

    return (<>
        <SectionHeader text="Create snapshot"/>
        <table>
            <tbody>
                <tr>
                    <td>Description</td>
                    <td>
                        <input
                            type={HtmlInputTypes.Text}
                            onChange={e => setDescription(e.target.value)}
                        />
                    </td>
                </tr>
            </tbody>
        </table>
        <br/>
        <IconButton
            icon={Icons.Snapshots.save}
            onClick={() => Controller.Snapshots.save(gameId, { description: description })}
            showTitle={true}
        />
    </>);
}