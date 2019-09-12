import * as React from 'react';
import { connect } from 'react-redux';
import Controller from '../../controllers/controller';
import { State } from '../../store/root';

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

const mapStateToProps = (_ : State) => {
    return {
        loadBoards: () => Controller.Game.loadAllBoards()
    };
}

const LoadAllBoards = connect(mapStateToProps)(loadAllBoards);
export default LoadAllBoards;