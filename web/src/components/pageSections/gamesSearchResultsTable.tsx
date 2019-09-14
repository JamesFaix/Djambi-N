import * as React from 'react';
import { Game } from '../../api/model';
import { SectionHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';
import Routes from '../../routes';
import IconBox from '../controls/iconBox';
import Copy from '../../utilities/copy';

const GamesSearchResultsTable : React.SFC<{ games : Game[] }> = props => {
    return (<>
        <SectionHeader text="Results"/>
        <table className={Classes.stripedTable}>
            <tbody>
                <tr>
                    <th></th>
                    <th>ID</th>
                    <th>Description</th>
                    <th>Created by</th>
                    <th>Status</th>
                    <th># Players</th>
                    <th># Regions</th>
                    <th>Is public</th>
                    <th>Allow guests</th>
                </tr>
                {props.games
                    .map((g, i) =>
                        <GameSearchResultsRow key={i} game={g}/>
                    )
                }
            </tbody>
        </table>
    </>);
}
export default GamesSearchResultsTable;

const GameSearchResultsRow : React.SFC<{ game : Game }> = props => {
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