import * as React from 'react';
import { Game, Effect, Board } from '../../../../api/model';
import {Kernel as K} from '../../../../kernel';

export interface HistoryEffectRowProps {
    game : Game,
    effect : Effect,
    getBoard : (regionCount : number) => Board
}

export default class HistoryEffectRow extends React.Component<HistoryEffectRowProps> {

    render() {
        return (
            <p>
                {K.copy.getEffectMessage(this.props.game, this.props.effect)}
            </p>
        );
    }
}