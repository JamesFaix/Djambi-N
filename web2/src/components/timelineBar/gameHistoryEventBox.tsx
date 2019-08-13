import * as React from 'react';
import { Event, Game } from "../../api/model";
import GameHistoryEffectBox from './gameHistoryEffectBox';
import Colors from '../../utilities/colors';

interface GameHistoryEventBoxProps {
    game : Game, //This will be needed eventually to get the correct player names from playerIDs in event objects
    event : Event
}

export default class GameHistoryEventBox extends React.Component<GameHistoryEventBoxProps> {
    render() {
        const e = this.props.event;
        const p = this.props.game.players.find(p => p.id === e.actingPlayerId);

        const borderStyle : React.CSSProperties = {
            borderStyle:"solid",
            borderWidth:1,
            borderColor:"gainsboro"
        };

        if (p) {
            const color = Colors.getColorFromPlayerColorId(p.colorId);
            borderStyle.boxShadow = `inset 0 0 0 3px ${color}`;
        }

        const agentName = this.getAgentName();

        return (
            <div style={borderStyle}>
                {`Event ${e.id} - ${e.createdBy.time}`}<br/>
                {`${agentName} did a ${e.kind}.`}<br/>
                Effects:<br/>
                <div style={{marginLeft:"10px"}}>
                    {e.effects.map((f, i) => {
                        return (
                            <GameHistoryEffectBox
                                game={this.props.game}
                                effect={f}
                                key={i}
                            />
                        );
                    })}
                </div>
            </div>
        );
    }

    private getAgentName() : string {
        const e = this.props.event;
        if (e.actingPlayerId) {
            return this.props.game.players.find(p => p.id === e.actingPlayerId).name;
        } else if (e.createdBy.userId) {
            return e.createdBy.userName;
        } else {
            return "System";
        }
    }
}