import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import GameStoreFlows from '../../storeFlows/game';

class loadAllBoards extends React.Component<{
    loadBoards : () => void
}> {
    componentDidMount() {
        this.props.loadBoards();
    }

    render() : JSX.Element {
        return null;
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        loadBoards: () => GameStoreFlows.loadAllBoards()(dispatch)
    };
}

const LoadAllBoards = connect(null, mapDispatchToProps)(loadAllBoards);
export default LoadAllBoards;