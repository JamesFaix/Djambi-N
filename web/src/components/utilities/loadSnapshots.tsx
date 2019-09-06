import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import SnapshotStoreFlows from '../../storeFlows/snapshots';

interface LoadSnapshotsProps {
    gameId : number,
    getSnapshots : (gameId : number) => void
}

class loadSnapshots extends React.Component<LoadSnapshotsProps> {
    componentDidMount() {
        this.props.getSnapshots(this.props.gameId);
    }

    render() : JSX.Element {
        return null;
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        getSnapshots : (gameId : number) => SnapshotStoreFlows.getSnapshots(gameId)(dispatch)
    };
}

const LoadSnapshots = connect(null, mapDispatchToProps)(loadSnapshots);
export default LoadSnapshots;