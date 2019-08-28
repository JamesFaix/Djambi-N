import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { GameParameters } from '../../api/model';
import Styles, { Classes } from '../../styles/styles';
import { SectionHeader } from '../controls/headers';
import ApiActions from '../../apiActions';
import * as StoreCreateGameForm from '../../store/createGameForm';

interface CreateGameFormProps {
    formData : GameParameters,
    onFormDataChanged : (formData: GameParameters) => void,
    submit : (formData: GameParameters) => void
}

class createGameForm extends React.Component<CreateGameFormProps> {
    render() {
        return (
            <div className={Classes.pageContainer}>
                <SectionHeader text="Game settings"/>
                <table className={Classes.borderlessTable}>
                    <tbody>
                        <tr>
                            <td>
                                Region count
                            </td>
                            <td>
                                <input
                                    type="number"
                                    min={3}
                                    max={8}
                                    value={this.props.formData.regionCount}
                                    onChange={e => this.onChangeRegionCount(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Allow guests
                            </td>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={this.props.formData.allowGuests}
                                    onChange={e => this.onChangeAllowGuests(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Public
                            </td>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={this.props.formData.isPublic}
                                    onChange={e => this.onChangeIsPublic(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Description
                            </td>
                            <td>
                                <input
                                    type="text"
                                    value={this.props.formData.description}
                                    onChange={e => this.onChangeDescription(e)}
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <button style={Styles.smallTopMargin()}
                    onClick={() => this.props.submit(this.props.formData)}
                >
                    Create game
                </button>
            </div>
        );
    }

    private onChangeRegionCount(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = parseInt(e.target.value);
        let formData = {...this.props.formData};
        formData.regionCount = value;
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
        submit: (formData: GameParameters) => ApiActions.createGame(formData)(dispatch)
    };
};

const CreateGameForm = connect(mapStateToProps, mapDispatchToProps)(createGameForm);

export default CreateGameForm;