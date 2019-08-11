import * as React from 'react';
import { GamesQuery, GameStatus } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import EnumDropdown from '../controls/enumDropdown';
import * as Actions from '../../store/actions';
import { Dispatch } from 'redux';

interface GamesSearchFormProps {
    query : GamesQuery,
    onQueryChanged : (query:GamesQuery) => void
}

class gamesSearchForm extends React.Component<GamesSearchFormProps> {

    private onChangeGameId(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = parseInt(e.target.value);
        let query = {...this.props.query};
        query.gameId = value;
        this.props.onQueryChanged(query);
    }

    private onChangeDescription(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.value;
        let query = {...this.props.query};
        query.descriptionContains = value;
        this.props.onQueryChanged(query);
    }

    private onChangeCreatedBy(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.value;
        let query = {...this.props.query};
        query.createdByUserName = value;
        this.props.onQueryChanged(query);
    }

    private onChangePlayerUserName(e : React.ChangeEvent<HTMLInputElement>) : void {
        const value = e.target.value;
        let query = {...this.props.query};
        query.playerUserName = value;
        this.props.onQueryChanged(query);
    }

    private onChangeIsPublic(value : boolean) : void {
        let query = {...this.props.query};
        query.isPublic = value;
        this.props.onQueryChanged(query);
    }

    private onChangeAllowGuests(value : boolean) : void {
        let query = {...this.props.query};
        query.allowGuests = value;
        this.props.onQueryChanged(query);
    }

    private onChangeStatus(value : GameStatus) : void {
        let query = {...this.props.query};
        query.status = value;
        this.props.onQueryChanged(query);
    }

    private emptyIfNull(value : any) : string {
        if (value === null) {
            return "";
        } else {
            return value + "";
        }
    }

    render() {
        const query = this.props.query;

        return (
            <div>
                <table>
                    <tbody>
                        <tr>
                            <td>GameId</td>
                            <td>
                                <input
                                    type="number"
                                    min={1}
                                    value={this.emptyIfNull(query.gameId)}
                                    onChange={e => this.onChangeGameId(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>Description</td>
                            <td>
                                <input
                                    type="text"
                                    value={this.emptyIfNull(query.descriptionContains)}
                                    onChange={e => this.onChangeDescription(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>Created by user</td>
                            <td>
                                <input
                                    type="text"
                                    value={this.emptyIfNull(query.createdByUserName)}
                                    onChange={e => this.onChangeCreatedBy(e)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td>Contains user</td>
                            <td>
                                <input
                                    type="text"
                                    value={this.emptyIfNull(query.playerUserName)}
                                    onChange={e => this.onChangePlayerUserName(e)}
                                />
                            </td>
                        </tr>
                        <tr>
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
                    </tbody>
                </table>
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        query: state.gamesQuery.query,
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onQueryChanged: (query: GamesQuery) => dispatch(Actions.updateGamesQuery(query))
    };
};

const GamesSearchForm = connect(mapStateToProps, mapDispatchToProps)(gamesSearchForm);

export default GamesSearchForm;