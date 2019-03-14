import * as React from 'react';
import Icon, { IconKind } from "./icon";
import { GameStatus } from "../../api/model";

interface IconDetails {
    hint : string,
    kind : IconKind
}

export interface GameStatusIconProps {
    status : GameStatus
}

export default class GameStatusIcon extends React.Component<GameStatusIconProps> {

    private getDetails(status : GameStatus) : IconDetails {
        switch (status) {
            case GameStatus.Pending:
                return {
                    hint: "Pending",
                    kind: IconKind.Pending,
                };
            case GameStatus.Started:
                return {
                    hint: "Started",
                    kind: IconKind.Started,
                };
            case GameStatus.Aborted:
                return {
                     hint: "Aborted",
                    kind: IconKind.Aborted,
                };
            case GameStatus.Finished:
                return {
                    hint: "Finished",
                    kind: IconKind.Finished,
                };
            default:
                throw "Invalid game status.";
        }
    }

    render() {
        const details = this.getDetails(this.props.status);

        return (
            <div>
                <Icon
                    kind={details.kind}
                    hint={details.hint}
                />
            </div>
        );
    }
}