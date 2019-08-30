import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import IconButton from '../controls/iconButton';
import * as Navigation from '../../store/navigation';
import { Icons } from '../../utilities/icons';

interface NavigationSectionProps {
    options : Navigation.State
}

class navigationSection extends React.Component<NavigationSectionProps> {
    render() {
        const o = this.props.options;
        return (
            <div>
                {o.enableSignup ?
                    <IconButton
                        icon={Icons.Pages.signup}
                        onClick={() => navigateTo(Routes.signup)}
                    />
                : null}
                {o.enableLogin ?
                    <IconButton
                        icon={Icons.Pages.login}
                        onClick={() => navigateTo(Routes.login)}
                    />
                : null}
                {o.enableDashboard ?
                    <IconButton
                        icon={Icons.Pages.home}
                        onClick={() => navigateTo(Routes.dashboard)}
                    />
                : null}
                {o.enableCreateGame ?
                    <IconButton
                        icon={Icons.Pages.newGame}
                        onClick={() => navigateTo(Routes.createGame)}
                    />
                : null}
                {o.enableLobby ?
                    <IconButton
                        icon={Icons.Pages.lobby}
                        onClick={() => navigateTo(Routes.lobby(o.gameId))}
                    />
                : null}
                {o.enablePlay ?
                    <IconButton
                        icon={Icons.Pages.play}
                        onClick={() => navigateTo(Routes.play(o.gameId))}
                    />
                : null}
                {o.enableDiplomacy ?
                    <IconButton
                        icon={Icons.Pages.diplomacy}
                        onClick={() => navigateTo(Routes.diplomacy(o.gameId))}
                    />
                : null}
            </div>
        );
    }
};

const mapStateToProps = (state : State) => {
    return {
        options: state.navigation
    };
};

const NavigationSection = connect(mapStateToProps)(navigationSection);
export default NavigationSection;