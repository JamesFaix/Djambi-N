import * as React from 'react';
import { Classes } from '../../styles/styles';
import { TimelineHeader } from '../controls/headers';
import { Icons } from '../../utilities/icons';
import Selectors from '../../selectors';
import { Player } from '../../api/model';
import Copy from '../../utilities/copy';
import { Game, User, TurnStatus } from '../../api/model';
import IconButton from '../controls/iconButton';
import Controller from '../../controllers/controller';

const CurrentTurnSection : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();
    const turn = game ? game.currentTurn : null;

    if (!turn || !user) { return null; }

    const currentPlayer = game.players.find(p => p.id === game.turnCycle[0]);
    const isCurrentUser = currentPlayer.userId === user.id;

    return (
        <div
            id="current-turn-section"
            className={Classes.timelineBarCurrentTurn}
        >
            <TimelineHeader icon={Icons.Timeline.currentTurn(currentPlayer.name, isCurrentUser)}/>
            <div
                className={Classes.playerBox}
                data-player-color-id={currentPlayer.colorId}
            >
                {isCurrentUser
                    ? <ActiveCurrentTurnSection/>
                    : <InactiveCurrentTurnSection player={currentPlayer}/>
                }
            </div>
        </div>
    );
}
export default CurrentTurnSection;

const ActiveCurrentTurnSection : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const turn = game.currentTurn;
    const board = Selectors.board(game.parameters.regionCount);
    return (<>
        {turn.selections.map((s, i) =>
            <p key={"row" + i}>
                {Copy.getSelectionDescription(s, game, board)}
            </p>)
        }
        <p style={{
            fontStyle:"italic",
            padding:"5px"
        }}>
            {`(${Copy.getTurnPrompt(turn)})`}
        </p>
        <CurrentTurnActionsBar/>
    </>);
}

const InactiveCurrentTurnSection : React.SFC<{
    player : Player
}> = props => {
    return (<>
        {`Waiting on ${props.player.name}...`}
    </>);
}

function isCurrentUser(user : User, game : Game) : boolean {
    return game.players
        .filter(p => p.userId === user.id)
        .map(p => p.id)
        .includes(game.turnCycle[0]);
}

const CurrentTurnActionsBar : React.SFC<{}> = _ => {
    return (
        <div style={{
            display: "flex"
        }}>
            <CommitButton/>
            <ResetButton/>
        </div>
    );
};

const ResetButton : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();
    const theme = Selectors.theme();
    const disabled = !isCurrentUser(user, game) ||
        game.currentTurn.selections.length === 0;

    return (
        <IconButton
            icon={Icons.PlayerActions.resetTurn}
            onClick={() => Controller.Game.resetTurn(game.id)}
            style={{
                backgroundColor: disabled ? null : theme.colors.negativeButtonBackground,
                flex: 1
            }}
            disabled={disabled}
        />
    );
};

const CommitButton : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();
    const theme = Selectors.theme();
    const disabled = !isCurrentUser(user, game) ||
        game.currentTurn.status !== TurnStatus.AwaitingCommit;

    return (
        <IconButton
            icon={Icons.PlayerActions.endTurn}
            onClick={() => Controller.Game.endTurn(game.id)}
            style={{
                backgroundColor: disabled ? null : theme.colors.positiveButtonBackground,
                flex: 1
            }}
            disabled={disabled}
        />
    );
};