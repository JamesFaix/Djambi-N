import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import BasicPageContainer from '../sections/basicPageContainer';
import { State } from '../../store/root';
import { connect } from 'react-redux';

interface SnapshotPageProps {
}

class snapshotsPage extends React.Component<SnapshotPageProps>{
    render() {
        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                Content
        </BasicPageContainer>
        );
    }
}

const mapStateToProps = (state : State) => {
    return {
    };
}

const SnapshotsPage = connect(mapStateToProps)(snapshotsPage)
export default SnapshotsPage;