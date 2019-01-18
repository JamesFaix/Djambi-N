import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User, GameParameters } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import LabeledInput from '../labeledInput';
import ActionButton from '../actionButton';
import Constants, { InputTypes } from '../../constants';

export interface CreateGamePageProps {
    user : User,
    api : ApiClient
}

export interface CreateGamePageState {
    regionCount : number,
    description : string,
    allowGuests : boolean,
    isPublic : boolean,
    gameId : number
}

export default class CreateGamePage extends React.Component<CreateGamePageProps, CreateGamePageState> {
    constructor(props : CreateGamePageProps){
        super(props);
        this.state = {
            regionCount : 3,
            description : "",
            allowGuests : true,
            isPublic : true,
            gameId : null
        }
    }

    private formOnChange(event : React.ChangeEvent<HTMLInputElement>) {
        const input = event.target;
        switch (input.name) {
            case "Region count":
                this.setState({ regionCount: Number(input.value) });
                break;

            case "Description":
                this.setState({ description: input.value });
                break;

            case "Allow guests":
                this.setState({ allowGuests: input.checked });
                break;

            case "Public":
                this.setState({ isPublic: input.checked });
                break;

            default:
                break;
        }
    }

    private submitOnClick() {
        const request : GameParameters = {
            regionCount: this.state.regionCount,
            description: this.state.description,
            allowGuests: this.state.allowGuests,
            isPublic: this.state.isPublic
        };

        this.props.api
            .createGame(request)
            .then(game => {
                this.setState({
                    regionCount : 3,
                    description : "",
                    allowGuests : true,
                    isPublic : true
                });

                this.setState({gameId : game.id});
            })
            .catch(reason => {
                alert("Create game failed because " + reason);
            });
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>;
        }

        //If lobby created, redirect to that lobby
        if (this.state.gameId !== null) {
            return <Redirect to={'/lobby/' + this.state.gameId}/>;
        }

        return (
            <div>
                <PageTitle label={"Create Game"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="My Games" to="/myGames"/>
                    <LinkButton label="Find Game" to="/findGame"/>
                </div>
                <br/>
                <br/>
                <div className="form">
                    <LabeledInput
                        label="Regions"
                        type={InputTypes.Number}
                        value={this.state.regionCount.toString()}
                        onChange={e => this.formOnChange(e)}
                        min={Constants.minRegionCount}
                        max={Constants.maxRegionCount}
                        tip="Number of sides on the board, and max number of players."
                    />
                    <br/>
                    <LabeledInput
                        label="Description"
                        type={InputTypes.Text}
                        placeholder="(Optional)"
                        value={this.state.description}
                        onChange={e => this.formOnChange(e)}
                        tip="A helpful note to idenity this game."
                    />
                    <br/>
                    <LabeledInput
                        label="Allow guests"
                        type={InputTypes.Checkbox}
                        checked={this.state.allowGuests}
                        onChange={e => this.formOnChange(e)}
                        tip="If checked, multiple players can share the same computer."
                    />
                    <br/>
                    <LabeledInput
                        label="Public"
                        type={InputTypes.Checkbox}
                        checked={this.state.isPublic}
                        onChange={e => this.formOnChange(e)}
                        tip="If checked, any user can join the game."
                    />
                    <br/>
                </div>
                <div className="centeredContainer">
                    <ActionButton label="Submit" onClick={() => this.submitOnClick()}/>
                </div>
            </div>
        );
    }
}