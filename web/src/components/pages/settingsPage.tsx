import * as React from 'react';
import BasicPageContainer from '../containers/basicPageContainer';
import ThemeFactory from '../../themes/themeFactory';
import Dropdown from '../controls/dropdown';
import { SectionHeader } from '../controls/headers';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';
import { useSelector } from 'react-redux';
import { State as AppState } from '../../store/root';
import { Checkbox, NumberInput } from '../controls/input';

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
                            items={items}
                            onChange={x => Controller.Display.changeTheme(x.name) }
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
                        <Checkbox
                            value={formData.showBoardTooltips}
                            onChange={x => onUpdate({ ...formData, showBoardTooltips: x })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Show cell and piece IDs</td>
                    <td>
                        <Checkbox
                            value={formData.showCellAndPieceIds}
                            onChange={x => onUpdate({ ...formData, showCellAndPieceIds: x })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Log API</td>
                    <td>
                        <Checkbox
                            value={formData.logApi}
                            onChange={x => onUpdate({ ...formData, logApi: x })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Log SSE</td>
                    <td>
                        <Checkbox
                            value={formData.logSse}
                            onChange={x => onUpdate({ ...formData, logSse: x })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Log Redux</td>
                    <td>
                        <Checkbox
                            value={formData.logRedux}
                            onChange={x => onUpdate({ ...formData, logRedux: x })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Seconds to display notifications</td>
                    <td>
                        <NumberInput
                            value={formData.showNotificationsSeconds}
                            onChange={x => onUpdate({ ...formData, showNotificationsSeconds: x })}
                            min={0}
                            max={300} //5min
                        />
                    </td>
                </tr>
            </tbody>
        </table>
    </>);
}