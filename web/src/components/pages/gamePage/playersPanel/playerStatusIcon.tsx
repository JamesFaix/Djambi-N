import * as React from 'react';
import Icon, { IconKind } from '../../../icon';
import { PlayerStatus } from '../../../../api/model';
import { Kernel as K } from '../../../../kernel';

interface IconDetails {
    hint : string,
    kind : IconKind,
    isActive : boolean
}

export interface PlayerStatusIconProps {
    status : PlayerStatus
}

export default class PlayerStatusIcon extends React.Component<PlayerStatusIconProps> {

    private getDetails(status : PlayerStatus) : IconDetails {
        switch (status) {
            case PlayerStatus.Pending:
                return {
                    hint: "Pending",
                    kind: IconKind.Pending,
                    isActive: false
                };
            case PlayerStatus.Alive:
                return {
                    hint: "Alive",
                    kind: IconKind.Alive,
                    isActive: true
                };
            case PlayerStatus.AcceptsDraw:
                return {
                     hint: "Will accept draw",
                    kind: IconKind.AcceptDraw,
                    isActive: true
                    };
            case PlayerStatus.Conceded:
                return {
                    hint: "Conceded",
                    kind: IconKind.Concede,
                    isActive: false
                };
            case PlayerStatus.WillConcede:
                return {
                    hint: "Will concede at the start of next turn",
                    kind: IconKind.Concede,
                    isActive: true
                };
            case PlayerStatus.Eliminated:
                return {
                    hint: "Eliminated",
                    kind: IconKind.Eliminated,
                    isActive: false
                 };
            case PlayerStatus.Victorious:
                return {
                     hint: "Victorious",
                     kind: IconKind.Victorious,
                     isActive: true
                     };
            default:
                throw "Invalid player status.";
        }
    }

    render() {
        const details = this.getDetails(this.props.status);
        const style = details.isActive
            ? undefined
            : {color: "silver"};

        return (
            <div style={style}>
                <Icon
                    kind={details.kind}
                    hint={details.hint}
                />
            </div>
        );
    }
}