import * as React from 'react';
import BasicPageContainer from '../sections/basicPageContainer';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import ThemeFactory from '../../themes/themeFactory';
import Dropdown from '../controls/dropdown';
import { SectionHeader } from '../controls/headers';
import { Theme } from '../../themes/model';
import DebugSettingsForm from '../forms/debugSettingsForm';
import Controller from '../../controller';
import ControllerEffects from '../../controllerEffects';
import { User } from '../../api/model';

const settingsPage : React.SFC<{
    user : User,
}> = props => {
    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);
    return(
        <BasicPageContainer>
            <SectionHeader text="Settings"/>
            <ThemeSection/>
            <DebugSettingsForm/>
        </BasicPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user : state.session.user
    };
}

const SettingsPage = connect(mapStateToProps)(settingsPage);
export default SettingsPage;

interface ThemeSectionProps {
    theme : Theme,
    selectTheme : (themeName : string) => void
}

class themeSection extends React.Component<ThemeSectionProps> {
    render() {
        const themes = ThemeFactory.getThemes();
        const items = Array.from(themes, ([key, value]) => {
            return { label: key, value: value };
        });

        return (<>
            <table>
                <tbody>
                    <tr>
                        <td>
                            Theme
                        </td>
                        <td>
                            <Dropdown
                                name="theme"
                                items={items}
                                onChange={(_: string, value : Theme) => this.props.selectTheme(value.name) }
                                currentValue={this.props.theme}
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
        </>);
    }
}

const mapStateToProps1 = (state : State) => {
    return {
        theme: state.display.theme,
        selectTheme: (themeName : string) => Controller.Display.changeTheme(themeName)
    };
}

const ThemeSection = connect(mapStateToProps1)(themeSection);