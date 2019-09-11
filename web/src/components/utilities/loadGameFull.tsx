import * as React from 'react';
import { connect } from 'react-redux';
import Controller from '../../controller';
import { State } from '../../store/root';

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

const mapStateToProps = (_ : State) => {
    return {
        loadGameFull : (gameId : number) => Controller.Game.loadGameFull(gameId)
    };
}

const LoadGameFull = connect(mapStateToProps)(loadGameFull);
export default LoadGameFull;