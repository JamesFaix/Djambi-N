import * as React from 'react';
import { GameStatus } from '../../api/model';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import EnumDropdown from '../controls/enumDropdown';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';

const GamesSearchForm : React.SFC<{}> = _ => {
    const query = useSelector((state : AppState) => state.gamesQuery.query);
    const onUpdate = Controller.Forms.updateGamesQuery;

    return (<>
        <SectionHeader text="Search games"/>
        <table>
            <tbody>
                <tr>
                    <td>Description</td>
                    <td>
                        <input
                            type={HtmlInputTypes.Text}
                            value={emptyIfNull(query.descriptionContains)}
                            onChange={e => onUpdate({ ...query, descriptionContains: nullIfEmpty(e.target.value) })}
                        />
                    </td>
                    <td>Is public</td>
                    <td>
                        <TristateDropdown
                            name={"IsPublic"}
                            value={query.isPublic}
                            onChange={(_, value) => onUpdate({ ...query, isPublic: value })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Created by user</td>
                    <td>
                        <input
                            type={HtmlInputTypes.Text}
                            value={emptyIfNull(query.createdByUserName)}
                            onChange={e => onUpdate({ ...query, createdByUserName: nullIfEmpty(e.target.value) })}
                        />
                    </td>
                    <td>Allow guests</td>
                    <td>
                        <TristateDropdown
                            name={"AllowGuests"}
                            value={query.allowGuests}
                            onChange={(_, value) => onUpdate({ ...query, allowGuests: value })}
                        />
                    </td>
                </tr>
                <tr>
                    <td>Contains user</td>
                    <td>
                        <input
                            type={HtmlInputTypes.Text}
                            value={emptyIfNull(query.playerUserName)}
                            onChange={e => onUpdate({ ...query, playerUserName: nullIfEmpty(e.target.value) })}
                        />
                    </td>
                    <td>Status</td>
                    <td>
                        <EnumDropdown
                            name={"Status"}
                            value={query.status}
                            onChange={(_, value) => onUpdate({ ...query, status: value })}
                            enum={GameStatus}
                        />
                    </td>
                </tr>
                <tr>
                    <td>GameId</td>
                    <td>
                        <input
                            style={{width:"50px"}}
                            type={HtmlInputTypes.Number}
                            min={1}
                            value={emptyIfNull(query.gameId)}
                            onChange={e => onUpdate({ ...query, gameId: parseInt(e.target.value) })}
                        />
                    </td>
                </tr>
            </tbody>
        </table>
        <br/>
        <IconButton
            icon={Icons.UserActions.search}
            showTitle={true}
            onClick={() => Controller.queryGames(query)}
        />
    </>);
};

function emptyIfNull(value : any) : string {
    return value === null ? "" : value;
}

function nullIfEmpty(value : string) : string {
    return value === "" ? null : value;
}

export default GamesSearchForm;