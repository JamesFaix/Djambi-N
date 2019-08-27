import * as React from 'react';
import { GamesQuery, GameStatus } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import EnumDropdown from '../controls/enumDropdown';
import * as Actions from '../../store/actions';
import { Dispatch } from 'redux';
import Styles from '../../styles/styles';
import { SectionHeader } from '../controls/headers';
import ApiActions from '../../apiActions';

interface GamesSearchFormProps {
    formData : GamesQuery,
    onFormDataChanged : (formData: GamesQuery) => void,
    submit : (formData : GamesQuery) => void
}

class gamesSearchForm extends React.Component<GamesSearchFormProps> {
    render() {
        const query = this.props.formData;

        return (
            <div style={Styles.pageContainer()}>
                <SectionHeader text="Search games"/>
                <table>
                    <tbody>
                        <tr>
                            <td style={Styles.noBorder()}>
                                Description
                            </td>
                            <td style={Styles.noBorder()}>
                                <input
                                    type="text"
                                    value={this.emptyIfNull(query.descriptionContains)}
                                    onChange={e => this.onChangeDescription(e)}
                                />
                            </td>
                            <td style={Styles.noBorder()}>
                                Is public
                            </td>
                            <td style={Styles.noBorder()}>
                                <TristateDropdown
                                    name={"IsPublic"}
                                    value={query.isPublic}
                                    onChange={(_, value) => this.onChangeIsPublic(value)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td style={Styles.noBorder()}>
                                Created by user
                            </td>
                            <td style={Styles.noBorder()}>
                                <input
                                    type="text"
                                    value={this.emptyIfNull(query.createdByUserName)}
                                    onChange={e => this.onChangeCreatedBy(e)}
                                />
                            </td>
                            <td style={Styles.noBorder()}>
                                Allow guests
                            </td>
                            <td style={Styles.noBorder()}>
                                <TristateDropdown
                                    name={"AllowGuests"}
                                    value={query.allowGuests}
                                    onChange={(_, value) => this.onChangeAllowGuests(value)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td style={Styles.noBorder()}>
                                Contains user
                            </td>
                            <td style={Styles.noBorder()}>
                                <input
                                    type="text"
                                    value={this.emptyIfNull(query.playerUserName)}
                                    onChange={e => this.onChangePlayerUserName(e)}
                                />
                            </td>
                            <td style={Styles.noBorder()}>
                                Status
                            </td>
                            <td style={Styles.noBorder()}>
                                <EnumDropdown
                                    name={"Status"}
                                    value={query.status}
                                    onChange={(_, value) => this.onChangeStatus(value)}
                                    enum={GameStatus}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td style={Styles.noBorder()}>
                                GameId
                            </td>
                            <td style={Styles.noBorder()}>
                                <input
                                    style={{width:"50px"}}
                                    type="number"
                                    min={1}
                                    value={this.emptyIfNull(query.gameId)}
                                    onChange={e => this.onChangeGameId(e)}
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <button style={Styles.smallTopMargin()}
                    onClick={() => this.props.submit(this.props.formData)}
                >
                    Search
                </button>
            </div>
        );
    }

    private emptyIfNull(value : any) : string {
        if (value === null) {
            return "";
        } else {
            return value + "";
        }
    }

    //#region Event handlers

    private onChangeGameId(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = parseInt(e.target.value);
        let query = {...this.props.formData};
        query.gameId = value;
        this.props.onFormDataChanged(query);
    }

    private onChangeDescription(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.value;
        let query = {...this.props.formData};
        query.descriptionContains = value;
        this.props.onFormDataChanged(query);
    }

    private onChangeCreatedBy(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.value;
        let query = {...this.props.formData};
        query.createdByUserName = value;
        this.props.onFormDataChanged(query);
    }

    private onChangePlayerUserName(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.value;
        let query = {...this.props.formData};
        query.playerUserName = value;
        this.props.onFormDataChanged(query);
    }

    private onChangeIsPublic(value : boolean) : void {
        let query = {...this.props.formData};
        query.isPublic = value;
        this.props.onFormDataChanged(query);
    }

    private onChangeAllowGuests(value : boolean) : void {
        let query = {...this.props.formData};
        query.allowGuests = value;
        this.props.onFormDataChanged(query);
    }

    private onChangeStatus(value : GameStatus) : void {
        let query = {...this.props.formData};
        query.status = value;
        this.props.onFormDataChanged(query);
    }

    //#endregion
}

const mapStateToProps = (state : AppState) => {
    return {
        formData: state.gamesQuery.query,
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onFormDataChanged: (formData: GamesQuery) => dispatch(Actions.updateGamesQuery(formData)),
        submit: (formData: GamesQuery) => ApiActions.queryGames(formData)(dispatch)
    };
};

const GamesSearchForm = connect(mapStateToProps, mapDispatchToProps)(gamesSearchForm);

export default GamesSearchForm;