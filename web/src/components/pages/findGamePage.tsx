import * as React from 'react';
import GamesQueryResultsTable from '../tables/gamesQueryResultsTable';
import LabeledInput from '../controls/labeledInput';
import LabeledTristateDropdown from '../controls/labeledTristateDropdown';
import PageTitle from '../pageTitle';
import Util from '../../util';
import {
    Game,
    GamesQuery,
    GameStatus,
    User
    } from '../../api/model';
import { InputTypes } from '../../constants';
import { Kernel as K } from '../../kernel';
import { Redirect } from 'react-router';
import Button, { ButtonKind } from '../controls/button';
import { IconKind } from '../icons/icon';
import { ResetButton, DashboardPageButton, MyGamesPageButton, CreateGamePageButton } from '../controls/navigationButtons';
import LabeledControl from '../controls/labeledControl';
import EnumDropdown from '../controls/enumDropdown';

export interface FindGamePageProps {
    user : User
}

export interface FindGamePageState {
    games : Game[],
    createdByUserNameFilter : string,
    playerUserNameFilter : string,
    isPublicFilter : boolean,
    allowGuestsFilter : boolean,
    descriptionContainsFilter : string,
    statusFilter : GameStatus
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
            descriptionContainsFilter: null,
            statusFilter: null
        };
    }

    public componentDidMount() : void {
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
            status: this.state.statusFilter
        }

        K.api
            .getGames(query)
            .then(games => {
                this.setState({games : games.reverse()});
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
                                <LabeledControl
                                    label="Status"
                                >
                                    <EnumDropdown
                                        enum={GameStatus}
                                        name="Status"
                                        value={this.state.statusFilter}
                                        onChange={(_, value) => this.setState({ statusFilter: value })}
                                    />
                                </LabeledControl>
                            </td>
                        </tr>
                        <tr>
                            <td className={K.classes.combine([K.classes.borderless, K.classes.rightAligned])}>
                                <Button
                                    kind={ButtonKind.Action}
                                    icon={IconKind.Find}
                                    onClick={() => this.refreshResults()}
                                    hint="Refresh results"
                                />
                            </td>
                            <td className={K.classes.borderless}>
                                <ResetButton
                                    onClick={() => this.resetOnClick()}
                                    hint="Clear filters"
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }

    public render() : JSX.Element {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to={K.routes.home()}/>
        }

        return (
            <div>
                <PageTitle label={"Find Game"}/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <DashboardPageButton/>
                    <MyGamesPageButton/>
                    <CreateGamePageButton/>
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