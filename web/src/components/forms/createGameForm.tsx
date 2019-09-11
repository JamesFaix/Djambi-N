import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { GameParameters } from '../../api/model';
import { SectionHeader } from '../controls/headers';
import * as StoreCreateGameForm from '../../store/createGameForm';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import GameStoreFlows from '../../storeFlows/game';
import HtmlInputTypes from '../htmlInputTypes';
import BoardSelectionBar from '../sections/boardSelectionBar';

interface CreateGameFormProps {
    formData : GameParameters,
    onFormDataChanged : (formData: GameParameters) => void,
    submit : (formData: GameParameters) => void
}

class createGameForm extends React.Component<CreateGameFormProps> {
    render() {
        return (<>
            <SectionHeader text="Board type"/>
            <BoardSelectionBar
                selectBoard={(regionCount : number) => this.onChangeRegionCount(regionCount)}
            />
            <br/>
            <SectionHeader text="Game settings"/>
            <table>
                <tbody>
                    <tr>
                        <td>Allow guests</td>
                        <td>
                            <input
                                type={HtmlInputTypes.CheckBox}
                                checked={this.props.formData.allowGuests}
                                onChange={e => this.onChangeAllowGuests(e)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Public</td>
                        <td>
                            <input
                                type={HtmlInputTypes.CheckBox}
                                checked={this.props.formData.isPublic}
                                onChange={e => this.onChangeIsPublic(e)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Description</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={this.props.formData.description}
                                onChange={e => this.onChangeDescription(e)}
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br/>
            <IconButton
                icon={Icons.UserActions.createGame}
                showTitle={true}
                onClick={() => this.props.submit(this.props.formData)}
            />
        </>);
    }

    private onChangeRegionCount(regionCount : number) : void {
        let formData = {...this.props.formData};
        formData.regionCount = regionCount;
        this.props.onFormDataChanged(formData);
    }

    private onChangeAllowGuests(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.checked;
        let formData = {...this.props.formData};
        formData.allowGuests = value;
        this.props.onFormDataChanged(formData);
    }

    private onChangeIsPublic(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.checked;
        let formData = {...this.props.formData};
        formData.isPublic = value;
        this.props.onFormDataChanged(formData);
    }

    private onChangeDescription(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.value;
        let formData = {...this.props.formData};
        formData.description = value;
        this.props.onFormDataChanged(formData);
    }
}

const mapStateToProps = (state : State) => {
    return {
        formData: state.createGameForm.parameters
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onFormDataChanged: (formData: GameParameters) => dispatch(StoreCreateGameForm.Actions.updateCreateGameForm(formData)),
        submit: (formData: GameParameters) => GameStoreFlows.createGame(formData)(dispatch)
    };
};

const CreateGameForm = connect(mapStateToProps, mapDispatchToProps)(createGameForm);

export default CreateGameForm;