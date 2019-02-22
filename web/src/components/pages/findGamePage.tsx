import * as React from 'react';
import PageTitle from '../pageTitle';
import { User, Game, GamesQuery, GameStatus } from '../../api/model';
import { Redirect } from 'react-router';
import LinkButton from '../controls/linkButton';
import LabeledInput from '../controls/labeledInput';
import { InputTypes } from '../../constants';
import LabeledTristateDropdown from '../controls/labeledTristateDropdown';
import ActionButton from '../controls/actionButton';
import Util from '../../util';
import GamesQueryResultsTable from '../gamesQueryResultsTable';
import {Kernel as K} from '../../kernel';

export interface FindGamePageProps {
    user : User
}

export interface FindGamePageState {
    games : Game[],
    createdByUserNameFilter : string,
    playerUserNameFilter : string,
    isPublicFilter : boolean,
    allowGuestsFilter : boolean,
    descriptionContainsFilter : string
}

export default class FindGamePage extends React.Component<FindGamePageProps, FindGamePageState> {
    constructor(props : FindGamePageProps) {
        super(props);
        this.state = {
            games : [],
            createdByUserNameFilter: null,
            playerUserNameFilter: null,
            isPublicFilter: null,
            allowGuestsFilter: null,
            descriptionContainsFilter: null
        };
    }

    componentDidMount() {
        this.refreshResults();
    }

    private refreshResults() {
        const query : GamesQuery = {
            gameId : null,
            createdByUserName: this.state.createdByUserNameFilter,
            playerUserName: this.state.playerUserNameFilter,
            isPublic: this.state.isPublicFilter,
            allowGuests: this.state.allowGuestsFilter,
            descriptionContains: this.state.descriptionContainsFilter,
            status: GameStatus.Pending //Find Games page only shows pending games that you can join
        }

        K.api
            .getGames(query)
            .then(games => {
                this.setState({games : games});
            })
            .catch(reason => {
                alert("Get games failed because " + reason);
            });
    }

//---Event handlers---

    private resetOnClick() {
        this.setState({
                createdByUserNameFilter: null,
                playerUserNameFilter: null,
                isPublicFilter: null,
                allowGuestsFilter: null,
                descriptionContainsFilter: null
            },
            () => this.refreshResults());
    }

//---Rendering---

    renderQueryFilters() {
        return (
            <div>
                <table className={K.classes.table}>
                    <tbody>
                        <tr>
                            <td className={K.classes.borderless}>
                                <LabeledInput
                                    label="Created by"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.createdByUserNameFilter)}
                                    onChange={e => this.setState({ createdByUserNameFilter: e.target.value })}
                                />
                            </td>
                            <td className={K.classes.borderless}>
                                <LabeledTristateDropdown
                                    label="Public"
                                    value={this.state.isPublicFilter}
                                    onChange={(_, value) => this.setState({ isPublicFilter: value })}
                                />
                            </td>
                        </tr>
                        <tr>
                        <td className={K.classes.borderless}>
                                <LabeledInput
                                    label="Has user"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.playerUserNameFilter)}
                                    onChange={e => this.setState({ playerUserNameFilter: e.target.value })}
                                />
                            </td>
                            <td className={K.classes.borderless}>
                                <LabeledTristateDropdown
                                    label="Guests allowed"
                                    value={this.state.allowGuestsFilter}
                                    onChange={(_, value) => this.setState({ allowGuestsFilter: value })}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td className={K.classes.borderless}>
                                <LabeledInput
                                    label="Description"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.descriptionContainsFilter)}
                                    onChange={e => this.setState({ descriptionContainsFilter: e.target.value })}
                                />
                            </td>
                            <td className={K.classes.borderless}>
                            </td>
                        </tr>
                        <tr>
                            <td className={K.classes.combine([K.classes.borderless, K.classes.rightAligned])}>
                                <ActionButton
                                    label="Search"
                                    onClick={() => this.refreshResults()}
                                />
                            </td>
                            <td className={K.classes.borderless}>
                                <ActionButton
                                    label="Reset"
                                    onClick={() => this.resetOnClick()}
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to={K.routes.home()}/>
        }

        return (
            <div>
                <PageTitle label={"Find Game"}/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <LinkButton label="Home" to={K.routes.dashboard()}/>
                    <LinkButton label="My Games" to={K.routes.myGames()}/>
                    <LinkButton label="Create Game" to={K.routes.createGame()}/>
                </div>
                <br/>
                {this.renderQueryFilters()}
                <br/>
                <GamesQueryResultsTable
                    games={this.state.games}
                />
            </div>
        );
    }
}