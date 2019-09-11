import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import Controller from '../../controller';

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

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        getSnapshots : (gameId : number) => Controller.Snapshots.get(gameId)
    };
}

const LoadSnapshots = connect(null, mapDispatchToProps)(loadSnapshots);
export default LoadSnapshots;