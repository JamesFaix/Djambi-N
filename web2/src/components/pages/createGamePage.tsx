import * as React from 'react';
import { GameParameters } from '../../api/model';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import * as Actions from '../../store/actions';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';

interface CreateGamePageProps {
    formData : GameParameters,
    onFormDataChanged : (formData: GameParameters) => void,
    onCreatGameClicked : (formData: GameParameters) => void
}

class createGamePage extends React.Component<CreateGamePageProps> {

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

    render() {
        return (
            <div>
                <RedirectToLoginIfNotLoggedIn/>
                <Link to={Routes.dashboard}>
                    <button>
                        Home
                    </button>
                </Link>
                <table>
                    <tbody>
                        <tr>
                            <td>Region count</td>
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
                            <td>Allow guests</td>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={this.props.formData.allowGuests}
                                    onChange={e => this.onChangeAllowGuests(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>Public</td>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={this.props.formData.isPublic}
                                    onChange={e => this.onChangeIsPublic(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>Description</td>
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
                <button
                    onClick={() => this.props.onCreatGameClicked(this.props.formData)}
                >
                    Create game
                </button>
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        formData: state.createGameForm.parameters
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onFormDataChanged: (formData: GameParameters) => dispatch(Actions.updateCreateGameForm(formData)),
        onCreatGameClicked: (formData: GameParameters) => ThunkActions.createGame(formData)(dispatch)
    };
};

const CreateGamePage = connect(mapStateToProps, mapDispatchToProps)(createGamePage);

export default CreateGamePage;