import * as React from 'react';
import { PlayHeader } from '../controls/headers';

export default class CurrentTurnSection extends React.Component<{}> {
    render() {
        return (
            <div>
                <PlayHeader text="Current turn"/>
            </div>
        );
    }
}