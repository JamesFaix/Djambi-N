import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User, Game, GamesQuery } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import LabeledInput from '../labeledInput';
import { InputTypes } from '../../constants';
import LabeledTristateDropdown from '../labeledTristateDropdown';
import ActionButton from '../actionButton';
import Util from '../../util';
import GameInfoTable from '../gameInfoTable';
import Routes from '../../routes';

export interface FindGamePageProps {
    user : User,
    api : ApiClient,
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
            descriptionContains: this.state.descriptionContainsFilter
        }

        this.props.api
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
                <table className="table">
                    <tbody>
                        <tr>
                            <td className="borderless">
                                <LabeledInput
                                    label="Created by"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.createdByUserNameFilter)}
                                    onChange={e => this.setState({ createdByUserNameFilter: e.target.value })}
                                />
                            </td>
                            <td className="borderless">
                                <LabeledTristateDropdown
                                    label="Public"
                                    value={this.state.isPublicFilter}
                                    onChange={(_, value) => this.setState({ isPublicFilter: value })}
                                />
                            </td>
                        </tr>
                        <tr>
                        <td className="borderless">
                                <LabeledInput
                                    label="Has user"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.playerUserNameFilter)}
                                    onChange={e => this.setState({ playerUserNameFilter: e.target.value })}
                                />
                            </td>
                            <td className="borderless">
                                <LabeledTristateDropdown
                                    label="Guests allowed"
                                    value={this.state.allowGuestsFilter}
                                    onChange={(_, value) => this.setState({ allowGuestsFilter: value })}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td className="borderless">
                                <LabeledInput
                                    label="Description"
                                    type={InputTypes.Text}
                                    value={Util.toStringSafe(this.state.descriptionContainsFilter)}
                                    onChange={e => this.setState({ descriptionContainsFilter: e.target.value })}
                                />
                            </td>
                            <td className="borderless">
                            </td>
                        </tr>
                        <tr>
                            <td className="borderless rightAligned">
                                <ActionButton
                                    label="Search"
                                    onClick={() => this.refreshResults()}
                                />
                            </td>
                            <td className="borderless">
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
            return <Redirect to={Routes.home()}/>
        }

        return (
            <div>
                <PageTitle label={"Find Game"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to={Routes.dashboard()}/>
                    <LinkButton label="My Games" to={Routes.myGames()}/>
                    <LinkButton label="Create Game" to={Routes.createGame()}/>
                </div>
                <br/>
                {this.renderQueryFilters()}
                <br/>
                <GameInfoTable
                    games={this.state.games}
                />
            </div>
        );
    }
}