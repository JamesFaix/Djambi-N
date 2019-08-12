import * as React from 'react';
import { Event, Game } from "../../api/model";

interface GameHistoryEventBoxProps {
    game : Game,
    event : Event
}

export default class GameHistoryEventBox extends React.Component<GameHistoryEventBoxProps> {
    render() {
        const borderStyle = {
            borderStyle:"solid",
            borderWidth:1,
            borderColor:"gainsboro"
        };
        return (
            <div style={borderStyle}>
                {`Event ${this.props.event.id}`}
                <br/>
                {`Player ${this.props.event.actingPlayerId} did something`}
            </div>
        );
    }
}