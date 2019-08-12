import * as React from 'react';
import { Game, Effect } from "../../api/model";

interface GameHistoryEffectBoxProps {
    game : Game, //This will be needed eventually to get the correct player names from playerIDs in effect objects
    effect : Effect
}

export default class GameHistoryEffectBox extends React.Component<GameHistoryEffectBoxProps> {
    render() {
        const f = this.props.effect;
        return (
            <div>
                {f.kind}<br/>
                {JSON.stringify(f.value)}
            </div>
        );
    }
}