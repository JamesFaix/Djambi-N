import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import GameStoreFlows from '../../storeFlows/game';

interface LoadGameFullProps {
    gameId : number,
    loadGameFull : (gameId : number) => void
}

class loadGameFull extends React.Component<LoadGameFullProps> {
    componentDidMount() {
        this.props.loadGameFull(this.props.gameId);
    }

    render() : JSX.Element {
        return null;
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        loadGameFull : (gameId : number) => GameStoreFlows.loadGameFull(gameId)(dispatch)
    };
}

const LoadGameFull = connect(null, mapDispatchToProps)(loadGameFull);

export default LoadGameFull;