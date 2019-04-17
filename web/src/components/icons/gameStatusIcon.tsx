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
            case GameStatus.InProgress:
                return {
                    hint: "In progress",
                    kind: IconKind.InProgress,
                };
            case GameStatus.Canceled:
                return {
                    hint: "Canceled",
                    kind: IconKind.Canceled,
                };
            case GameStatus.Over:
                return {
                    hint: "Over",
                    kind: IconKind.Over,
                };
            default:
                throw "Invalid game status.";
        }
    }

    public render() : JSX.Element {
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