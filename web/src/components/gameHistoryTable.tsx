import * as React from 'react';
import { Game, Event } from '../api/model';
import ThemeService from '../themes/themeService';

export interface GameHistoryTableProps {
    game : Game,
    events : Event[],
    theme : ThemeService
}

export default class GameHistoryTable extends React.Component<GameHistoryTableProps> {

    render() {
        return (
            <div style={{display:"flex"}}>
                <table className="table">
                    <tbody>

                    </tbody>
                </table>
            </div>
        );
    }
}