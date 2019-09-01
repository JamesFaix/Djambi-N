import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import BasicPageContainer from '../sections/basicPageContainer';
import * as StoreNavigation from '../../store/navigation';
import { User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';

interface SnapshotPageProps {
    user : User
}

class snapshotsPage extends React.Component<SnapshotPageProps>{
    render() {
        const gameId = (this.props as any).match.params.gameId;

        const navOptions = {
            enableDashboard: true,
            enableLobby: true,
            gameId: gameId,
            enablePlay: true
        };

        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <SetNavigationOptions options={navOptions}/>
                Content
        </BasicPageContainer>
        );
    }
}

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user
    };
}

const SnapshotsPage = connect(mapStateToProps)(snapshotsPage)
export default SnapshotsPage;