import * as React from 'react';
import { Game } from '../../api/model';
import IconButton from '../controls/iconButton';
import { Classes } from '../../styles/styles';
import IconBox from '../controls/iconBox';
import { Icons } from '../../utilities/icons';
import Copy from '../../utilities/copy';
import Controller from '../../controller';
import Routes from '../../routes';

const GameSearchResultsRow : React.SFC<{
    game : Game
}> = props => {
    const game = props.game;
    return (
        <tr>
            <td>
                <IconButton
                    icon={Icons.UserActions.loadGame}
                    onClick={() => Controller.navigateTo(Routes.game(game.id))}
                />
            </td>
            <td className={Classes.centered}>
                {game.id}
            </td>
            <td>
                {game.parameters.description}
            </td>
            <td>
                {game.createdBy.userName}
            </td>
            <td className={Classes.centered}>
                <IconBox icon={Icons.gameStatus(game.status)}/>
            </td>
            <td className={Classes.centered}>
                {game.players.length}
            </td>
            <td className={Classes.centered}>
                {game.parameters.regionCount}
            </td>
            <td className={Classes.centered}>
                {Copy.boolToYesOrNo(game.parameters.isPublic)}
            </td>
            <td className={Classes.centered}>
                {Copy.boolToYesOrNo(game.parameters.allowGuests)}
            </td>
        </tr>
    );
}
export default GameSearchResultsRow;