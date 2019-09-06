import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../sections/basicPageContainer';
import SnapshotsTable from '../tables/snapshotsTable';
import CreateSnapshotForm from '../forms/createSnapshotForm';
import LoadSnapshots from '../utilities/loadSnapshots';

export default class SnapshotsPage extends React.Component<{}>{
    render() {
        const gameId = (this.props as any).match.params.gameId;
        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <LoadSnapshots gameId={gameId}/>
                <SnapshotsTable/>
                <br/>
                <br/>
                <CreateSnapshotForm/>
            </BasicPageContainer>
        );
    }
}