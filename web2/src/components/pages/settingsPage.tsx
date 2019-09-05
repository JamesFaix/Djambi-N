import * as React from 'react';
import BasicPageContainer from '../sections/basicPageContainer';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import ThemeFactory from '../../themes/themeFactory';
import { Dispatch } from 'redux';
import Dropdown from '../controls/dropdown';
import { SectionHeader } from '../controls/headers';
import ThemeService from '../../themes/themeService';
import { Theme } from '../../themes/model';

interface SettingsPageProps {
}

export default class SettingsPage extends React.Component<SettingsPageProps> {
    render(){
        return(
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <SectionHeader text="Settings"/>
                <ThemeSection/>
            </BasicPageContainer>
        );
    }
}

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

const mapStateToProps = (state : State) => {
    return {
        theme: state.display.theme
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        selectTheme: (themeName : string) => ThemeService.changeTheme(themeName, dispatch)
    };
}

const ThemeSection = connect(mapStateToProps, mapDispatchToProps)(themeSection);
