import * as React from 'react';
import { NavigationState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as Actions from '../../store/actions';

interface SetNavigationOptionsProps {
    options : NavigationState,
    setOptions : (options : NavigationState) => void
}

class setNavigationOptions extends React.Component<SetNavigationOptionsProps> {
    componentDidMount() {
        this.props.setOptions(this.props.options);
    }

    render() : JSX.Element {
        return null;
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        setOptions: (options: NavigationState) => dispatch(Actions.setNavigationOptions(options))
    };
}

const SetNavigationOptions = connect(null, mapDispatchToProps)(setNavigationOptions);

export default SetNavigationOptions;