import * as React from 'react';
import ApiClient from '../../api/client';
import { User } from '../../api/model';
import LinkButton from '../linkButton';
import PageTitle from '../pageTitle';
import GameBoard from '../gameBoard';

 export interface GamePageProps {
    user : User,
    api : ApiClient,
    gameId : number,
    rulesUrl : string
}

 export interface GamePageState {

 }

 export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    constructor(props : GamePageProps) {
        super(props);
        this.state = {

         };
    }

     render() {
        return (
            <div>
                <PageTitle label={"Game"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="Rules" to={this.props.rulesUrl} newWindow={true}/>
                </div>
                <br/>
                <GameBoard
                    user={this.props.user}
                    api={this.props.api}
                    gameId={this.props.gameId}
                />
            </div>
        );
    }
}