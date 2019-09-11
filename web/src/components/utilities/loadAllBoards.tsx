import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import Controller from '../../controller';

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

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        loadBoards: () => Controller.Game.loadAllBoards()
    };
}

const LoadAllBoards = connect(null, mapDispatchToProps)(loadAllBoards);
export default LoadAllBoards;