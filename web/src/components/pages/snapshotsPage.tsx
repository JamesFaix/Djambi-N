import * as React from 'react';
import BasicPageContainer from '../sections/basicPageContainer';
import SnapshotsTable from '../tables/snapshotsTable';
import CreateSnapshotForm from '../forms/createSnapshotForm';
import LoadSnapshots from '../utilities/loadSnapshots';
import { User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import ControllerEffects from '../../controllerEffects';

const snapshotsPage : React.SFC<{
    user : User
}> = props => {
    const gameId = (props as any).match.params.gameId;

    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);

    return (
        <BasicPageContainer>
            <LoadSnapshots gameId={gameId}/>
            <SnapshotsTable/>
            <br/>
            <br/>
            <CreateSnapshotForm/>
        </BasicPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user : state.session.user
    };
}

const SnapshotsPage = connect(mapStateToProps)(snapshotsPage);
export default SnapshotsPage;