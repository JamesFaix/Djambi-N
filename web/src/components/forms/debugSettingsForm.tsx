import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import { DebugSettings } from '../../debug';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controller';

interface DebugSettingsFormProps {
    formData : DebugSettings,
    onFormDataChanged : (settings:DebugSettings) => void
}

class debugSettingsForm extends React.Component<DebugSettingsFormProps> {
    render() {
        const s = this.props.formData;

        return (<>
            <SectionHeader text="Debug settings"/>
            <table>
                <tbody>
                    <tr>
                        <td>Show board tooltips</td>
                        <td>
                            <input
                                type={HtmlInputTypes.CheckBox}
                                checked={s.showBoardTooltips}
                                onChange={e => this.onChangeShowBoardTooltips(e)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Show cell and piece IDs</td>
                        <td>
                            <input
                                type={HtmlInputTypes.CheckBox}
                                checked={s.showCellAndPieceIds}
                                onChange={e => this.onChangeShowCellAndPieceIds(e)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Log API</td>
                        <td>
                            <input
                                type={HtmlInputTypes.CheckBox}
                                checked={s.logApi}
                                onChange={e => this.onChangeLogApi(e)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Log SSE</td>
                        <td>
                            <input
                                type={HtmlInputTypes.CheckBox}
                                checked={s.logSse}
                                onChange={e => this.onChangeLogSse(e)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Log Redux</td>
                        <td>
                            <input
                                type={HtmlInputTypes.CheckBox}
                                checked={s.logRedux}
                                onChange={e => this.onChangeLogRedux(e)}
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
        </>);
    }

    private onChangeShowBoardTooltips(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.checked;
        const formData = {
            ...this.props.formData,
            showCellLabels: value
        };
        this.props.onFormDataChanged(formData);
    }

    private onChangeShowCellAndPieceIds(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.checked;
        const formData = {
            ...this.props.formData,
            showCellAndPieceIds: value
        };
        this.props.onFormDataChanged(formData);
    }

    private onChangeLogApi(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.checked;
        const formData = {
            ...this.props.formData,
            logApi: value
        };
        this.props.onFormDataChanged(formData);
    }

    private onChangeLogSse(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.checked;
        const formData = {
            ...this.props.formData,
            logSse: value
        };
        this.props.onFormDataChanged(formData);
    }

    private onChangeLogRedux(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.checked;
        const formData = {
            ...this.props.formData,
            logRedux: value
        };
        this.props.onFormDataChanged(formData);
    }
}

const mapStateToProps = (state : State) => {
    return {
        formData: state.settings.debug,
        onFormDataChanged: (formData: DebugSettings) => Controller.Settings.saveAndApply(formData),
    };
};

const DebugSettingsForm = connect(mapStateToProps)(debugSettingsForm);
export default DebugSettingsForm;