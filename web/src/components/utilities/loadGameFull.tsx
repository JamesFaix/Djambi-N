import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import Controller from '../../controller';

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

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        loadGameFull : (gameId : number) => Controller.Game.loadGameFull(gameId)
    };
}

const LoadGameFull = connect(null, mapDispatchToProps)(loadGameFull);

export default LoadGameFull;