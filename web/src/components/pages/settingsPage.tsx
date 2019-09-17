import * as React from 'react';
import BasicPageContainer from '../containers/basicPageContainer';
import ThemeFactory from '../../themes/themeFactory';
import Dropdown from '../controls/dropdown';
import { SectionHeader } from '../controls/headers';
import { Theme } from '../../themes/model';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';
import { useSelector } from 'react-redux';
import HtmlInputTypes from '../htmlInputTypes';
import { State as AppState } from '../../store/root';

const SettingsPage : React.SFC<{}> = _ => {
    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn();
    });

    return(
        <BasicPageContainer>
            <SectionHeader text="Settings"/>
            <ThemeSelector/>
            <DebugSettingsForm/>
        </BasicPageContainer>
    );
}
export default SettingsPage;

const ThemeSelector : React.SFC<{}> = _ => {
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
                            onChange={(_: string, value : Theme) => Controller.Display.changeTheme(value.name) }
                            currentValue={Selectors.theme()}
                        />
                    </td>
                </tr>
            </tbody>
        </table>
    </>);
}

const DebugSettingsForm : React.SFC<{}> = _ => {
    const onUpdate = Controller.Settings.saveAndApply;
    const formData = useSelector((state : AppState) => state.settings.debug);

    return (<>
        <SectionHeader text="Debug settings"/>
        <table>
            <tbody>
                <tr>
                    <td>Show board tooltips</td>
                    <td>
                        <input
                            type={HtmlInputTypes.CheckBox}
                            checked={formData.showBoardTooltips}
                            onChange={e => onUpdate({ ...formData, showBoardTooltips: e.target.checked })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Show cell and piece IDs</td>
                    <td>
                        <input
                            type={HtmlInputTypes.CheckBox}
                            checked={formData.showCellAndPieceIds}
                            onChange={e => onUpdate({ ...formData, showCellAndPieceIds: e.target.checked })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Log API</td>
                    <td>
                        <input
                            type={HtmlInputTypes.CheckBox}
                            checked={formData.logApi}
                            onChange={e => onUpdate({ ...formData, logApi: e.target.checked })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Log SSE</td>
                    <td>
                        <input
                            type={HtmlInputTypes.CheckBox}
                            checked={formData.logSse}
                            onChange={e => onUpdate({ ...formData, logSse: e.target.checked })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Log Redux</td>
                    <td>
                        <input
                            type={HtmlInputTypes.CheckBox}
                            checked={formData.logRedux}
                            onChange={e => onUpdate({ ...formData, logRedux: e.target.checked })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Seconds to display notifications</td>
                    <td>
                        <input
                            type={HtmlInputTypes.Number}
                            value={formData.showNotificationsSeconds}
                            onChange={e => onUpdate({ ...formData, showNotificationsSeconds: Number(e.target.value) })}
                            min={0}
                            max={300} //5min
                        />
                    </td>
                </tr>
            </tbody>
        </table>
    </>);
}