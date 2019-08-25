import * as React from 'react';
import { Dispatch } from 'redux';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { Game, User, TurnStatus } from '../../api/model';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faHandshake, faRecycle, faCheck } from '@fortawesome/free-solid-svg-icons';
import * as ThunkActions from '../../thunkActions';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import Styles from '../../styles/styles';

interface CurrentTurnActionsBarProps {
    user : User,
    game : Game,
    endTurn : (gameId : number) => void,
    resetTurn : (gameId : number) => void
}

class currentTurnActionsBar extends React.Component<CurrentTurnActionsBarProps> {
    render() {
        return (
            <div>
                {this.renderEndButton()}
                {this.renderResetButton()}
                {this.renderDiplomacyButton()}
            </div>
        );
    }

    private isCurrentUsersTurn() : boolean {
        const userId = this.props.user.id;
        const playerIds = this.props.game.players
            .filter(p => p.userId === userId)
            .map(p => p.id);
        const currentPlayerId = this.props.game.turnCycle[0];
        return playerIds.includes(currentPlayerId);
    }

    private renderResetButton(){
        if (!this.isCurrentUsersTurn() ||
            this.props.game.currentTurn.selections.length === 0
        ) {
            return null;
        }

        return (
            <button
                style={Styles.iconButton()}
                onClick={() => this.props.resetTurn(this.props.game.id)}
                title={"Reset turn"}
            >
                <FontAwesomeIcon icon={faRecycle}/>
            </button>
        );
    }

    private renderEndButton() {
        if (!this.isCurrentUsersTurn() ||
            this.props.game.currentTurn.status !== TurnStatus.AwaitingCommit
        ) {
            return null;
        }

        return (
            <button
                style={Styles.iconButton()}
                onClick={() => this.props.endTurn(this.props.game.id)}
                title={"Finish turn"}
            >
                <FontAwesomeIcon icon={faCheck}/>
            </button>
        );
    }

    private renderDiplomacyButton() {
        return (
            <button
                title={"Concede or Draw"}
                style={Styles.iconButton()}
            >
                <Link to={Routes.diplomacy(this.props.game.id)}>
                    <FontAwesomeIcon icon={faHandshake}/>
                </Link>
            </button>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        user: state.session.user,
        game: state.activeGame.game
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        endTurn: (gameId : number) => ThunkActions.endTurn(gameId)(dispatch),
        resetTurn: (gameId : number) => ThunkActions.resetTurn(gameId)(dispatch)
    };
}

const CurrentTurnActionsBar = connect(mapStateToProps, mapDispatchToProps)(currentTurnActionsBar);
export default CurrentTurnActionsBar;