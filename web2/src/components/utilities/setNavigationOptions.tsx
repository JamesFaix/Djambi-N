import * as React from 'react';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as Navigation from '../../store/navigation';

interface SetNavigationOptionsProps {
    options : Navigation.State,
    setOptions : (options : Navigation.State) => void
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
        setOptions: (options: Navigation.State) => dispatch(Navigation.Actions.setNavigationOptions(options))
    };
}

const SetNavigationOptions = connect(null, mapDispatchToProps)(setNavigationOptions);

export default SetNavigationOptions;