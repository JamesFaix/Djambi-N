import * as React from 'react';
import { Dispatch } from 'redux';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Game, User, TurnStatus } from '../../api/model';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import GameStoreFlows from '../../storeFlows/game';
import { Theme } from '../../themes/model';

function isCurrentUser(user : User, game : Game) : boolean {
    return game.players
        .filter(p => p.userId === user.id)
        .map(p => p.id)
        .includes(game.turnCycle[0]);
}

const ResetButton : React.SFC<{
    game : Game,
    user : User,
    theme : Theme,
    onClick : () => void
}> = props => {
    const disabled = !isCurrentUser(props.user, props.game) ||
        props.game.currentTurn.selections.length === 0;

    return (
        <IconButton
            icon={Icons.PlayerActions.resetTurn}
            onClick={() => props.onClick()}
            style={{
                backgroundColor: disabled ? null : props.theme.colors.negativeButtonBackground,
                flex: 1
            }}
            disabled={disabled}
        />
    );
};

const CommitButton : React.SFC<{
    game : Game,
    user : User,
    theme : Theme,
    onClick : () => void
}> = props => {
    const disabled = !isCurrentUser(props.user, props.game) ||
        props.game.currentTurn.status !== TurnStatus.AwaitingCommit;

    return (
        <IconButton
            icon={Icons.PlayerActions.endTurn}
            onClick={() => props.onClick()}
            style={{
                backgroundColor: disabled ? null : props.theme.colors.positiveButtonBackground,
                flex: 1
            }}
            disabled={disabled}
        />
    );
};

const currentTurnActionsBar : React.SFC<{
    user : User,
    game : Game,
    endTurn : (gameId : number) => void,
    resetTurn : (gameId : number) => void,
    theme : Theme
}> = props => (
    <div style={{
        display: "flex"
    }}>
        <CommitButton
            game={props.game}
            user={props.user}
            theme={props.theme}
            onClick={() => props.endTurn(props.game.id)}
        />
        <ResetButton
            game={props.game}
            user={props.user}
            theme={props.theme}
            onClick={() => props.resetTurn(props.game.id)}
        />
    </div>
);

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game,
        theme: state.display.theme
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        endTurn: (gameId : number) => GameStoreFlows.endTurn(gameId)(dispatch),
        resetTurn: (gameId : number) => GameStoreFlows.resetTurn(gameId)(dispatch)
    };
}

const CurrentTurnActionsBar = connect(mapStateToProps, mapDispatchToProps)(currentTurnActionsBar);
export default CurrentTurnActionsBar;