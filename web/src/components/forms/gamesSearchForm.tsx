import * as React from 'react';
import { GamesQuery, GameStatus } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import EnumDropdown from '../controls/enumDropdown';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';

interface GamesSearchFormProps {
    formData : GamesQuery,
    onFormDataChanged : (formData: GamesQuery) => void,
    submit : (formData : GamesQuery) => void
}

class gamesSearchForm extends React.Component<GamesSearchFormProps> {
    render() {
        const query = this.props.formData;

        return (<>
            <SectionHeader text="Search games"/>
            <table>
                <tbody>
                    <tr>
                        <td>Description</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={this.emptyIfNull(query.descriptionContains)}
                                onChange={e => this.onChangeDescription(e)}
                            />
                        </td>
                        <td>Is public</td>
                        <td>
                            <TristateDropdown
                                name={"IsPublic"}
                                value={query.isPublic}
                                onChange={(_, value) => this.onChangeIsPublic(value)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Created by user</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={this.emptyIfNull(query.createdByUserName)}
                                onChange={e => this.onChangeCreatedBy(e)}
                            />
                        </td>
                        <td>Allow guests</td>
                        <td>
                            <TristateDropdown
                                name={"AllowGuests"}
                                value={query.allowGuests}
                                onChange={(_, value) => this.onChangeAllowGuests(value)}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Contains user</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={this.emptyIfNull(query.playerUserName)}
                                onChange={e => this.onChangePlayerUserName(e)}
                            />
                        </td>
                        <td>Status</td>
                        <td>
                            <EnumDropdown
                                name={"Status"}
                                value={query.status}
                                onChange={(_, value) => this.onChangeStatus(value)}
                                enum={GameStatus}
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>GameId</td>
                        <td>
                            <input
                                style={{width:"50px"}}
                                type={HtmlInputTypes.Number}
                                min={1}
                                value={this.emptyIfNull(query.gameId)}
                                onChange={e => this.onChangeGameId(e)}
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br/>
            <IconButton
                icon={Icons.UserActions.search}
                showTitle={true}
                onClick={() => this.props.submit(this.props.formData)}
            />
        </>);
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
        let value = e.target.value;
        if (value === "") value = null;
        let query = {...this.props.formData};
        query.descriptionContains = value;
        this.props.onFormDataChanged(query);
    }

    private onChangeCreatedBy(e : React.ChangeEvent<HTMLInputElement>) : void {
        let value = e.target.value;
        if (value === "") value = null;
        let query = {...this.props.formData};
        query.createdByUserName = value;
        this.props.onFormDataChanged(query);
    }

    private onChangePlayerUserName(e : React.ChangeEvent<HTMLInputElement>) : void {
        let value = e.target.value;
        if (value === "") value = null;
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

const mapStateToProps = (state : State) => {
    return {
        formData: state.gamesQuery.query,
        onFormDataChanged: (formData: GamesQuery) => Controller.Forms.updateGamesQuery(formData),
        submit: (formData: GamesQuery) => Controller.queryGames(formData)
    };
};

const GamesSearchForm = connect(mapStateToProps)(gamesSearchForm);
export default GamesSearchForm;