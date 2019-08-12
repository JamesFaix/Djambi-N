import * as React from 'react';
import { Event, Game } from "../../api/model";
import GameHistoryEffectBox from './gameHistoryEffectBox';

interface GameHistoryEventBoxProps {
    game : Game, //This will be needed eventually to get the correct player names from playerIDs in event objects
    event : Event
}

export default class GameHistoryEventBox extends React.Component<GameHistoryEventBoxProps> {
    render() {
        const borderStyle = {
            borderStyle:"solid",
            borderWidth:1,
            borderColor:"gainsboro"
        };

        const e = this.props.event;

        return (
            <div style={borderStyle}>
                {`Event ${e.id} at ${e.createdBy.time}`}<br/>
                {`Player ${e.actingPlayerId} did a ${e.kind}.`}<br/>
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
}