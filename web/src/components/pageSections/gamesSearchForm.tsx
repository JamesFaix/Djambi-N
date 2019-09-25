import * as React from 'react';
import { GameStatus } from '../../api/model';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import { IconSubmitButton } from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';
import DateService from '../../utilities/dates';

const GamesSearchForm : React.SFC<{}> = _ => {
    const query = useSelector((state : AppState) => state.search.query);
    const onUpdate = Controller.Forms.updateGamesQuery;

    return (
        <form
            onSubmit={() => Controller.Search.searchGames(query)}
        >
            <div id="row1" style={{display:"flex"}}>
                <div>
                    GameId
                    <input
                        style={{width:"50px"}}
                        type={HtmlInputTypes.Number}
                        min={1}
                        value={emptyIfNull(query.gameId)}
                        onChange={e => onUpdate({ ...query, gameId: parseInt(e.target.value) })}
                    />
                </div>
                <div>
                    Description
                    <input
                        type={HtmlInputTypes.Text}
                        value={emptyIfNull(query.descriptionContains)}
                        onChange={e => onUpdate({ ...query, descriptionContains: nullIfEmpty(e.target.value) })}
                        autoFocus
                    />
                </div>
            </div>
            <div id="row2" style={{display:"flex"}}>
                <div>
                    Created by user
                    <input
                        type={HtmlInputTypes.Text}
                        value={emptyIfNull(query.createdByUserName)}
                        onChange={e => onUpdate({ ...query, createdByUserName: nullIfEmpty(e.target.value) })}
                    />
                </div>
                <div>
                    Contains user
                    <input
                        type={HtmlInputTypes.Text}
                        value={emptyIfNull(query.playerUserName)}
                        onChange={e => onUpdate({ ...query, playerUserName: nullIfEmpty(e.target.value) })}
                    />
                </div>
            </div>
            <div id="row3" style={{display:"flex"}}>
                <div>
                    Is public
                    <TristateDropdown
                        name={"IsPublic"}
                        value={query.isPublic}
                        onChange={(_, value) => onUpdate({ ...query, isPublic: value })}
                    />
                </div>
                <div>
                    Allow guests
                    <TristateDropdown
                        name={"AllowGuests"}
                        value={query.allowGuests}
                        onChange={(_, value) => onUpdate({ ...query, allowGuests: value })}
                    />
                </div>
            </div>
            <div id="row4" style={{display:"flex"}}>
                Pending
                <input
                    type={HtmlInputTypes.CheckBox}
                    checked={query.statuses.includes(GameStatus.Pending)}
                    onChange={e => {
                        const statuses = e.target.checked
                            ? query.statuses.concat([GameStatus.Pending])
                            : query.statuses.filter(x => x === GameStatus.Pending);
                        onUpdate({ ...query, statuses: statuses});
                    }}
                />
                In Progress
                <input
                    type={HtmlInputTypes.CheckBox}
                    checked={query.statuses.includes(GameStatus.InProgress)}
                    onChange={e => {
                        const statuses = e.target.checked
                            ? query.statuses.concat([GameStatus.InProgress])
                            : query.statuses.filter(x => x === GameStatus.InProgress);
                        onUpdate({ ...query, statuses: statuses});
                    }}
                />
                Over
                <input
                    type={HtmlInputTypes.CheckBox}
                    checked={query.statuses.includes(GameStatus.Over)}
                    onChange={e => {
                        const statuses = e.target.checked
                            ? query.statuses.concat([GameStatus.Over])
                            : query.statuses.filter(x => x === GameStatus.Over);
                        onUpdate({ ...query, statuses: statuses});
                    }}
                />
                Canceled
                <input
                    type={HtmlInputTypes.CheckBox}
                    checked={query.statuses.includes(GameStatus.Canceled)}
                    onChange={e => {
                        const statuses = e.target.checked
                            ? query.statuses.concat([GameStatus.Canceled])
                            : query.statuses.filter(x => x === GameStatus.Canceled);
                        onUpdate({ ...query, statuses: statuses});
                    }}
                />
            </div>
            <div id="row5" style={{display:"flex"}}>
                Created between
                <DatePicker
                    value={query.createdAfter}
                    onUpdate={d => onUpdate({ ...query, createdAfter: d })}
                    isStartDate={true}
                />
                and
                <DatePicker
                    value={query.createdBefore}
                    onUpdate={d => onUpdate({ ...query, createdBefore: d })}
                    isStartDate={false}
                />
            </div>
            <div id="row6"> style={{display:"flex"}}
                Last event between
                <DatePicker
                    value={query.lastEventAfter}
                    onUpdate={d => onUpdate({ ...query, lastEventAfter: d })}
                    isStartDate={true}
                />
                and
                <DatePicker
                    value={query.lastEventBefore}
                    onUpdate={d => onUpdate({ ...query, lastEventBefore: d })}
                    isStartDate={false}
                />
            </div>
            <br/>
            <IconSubmitButton
                icon={Icons.UserActions.search}
                showTitle={true}
            />
        </form>
    );
};
export default GamesSearchForm;

function emptyIfNull(value : any) : string {
    return value === null ? "" : value;
}

function nullIfEmpty(value : string) : string {
    return value === "" ? null : value;
}

const DatePicker : React.SFC<{
    value: Date,
    onUpdate: (d:Date) => void,
    isStartDate : boolean
}> = props => {
    return (
        <input
            type={HtmlInputTypes.Date}
            value={DateService.dateToDatepickerString(props.value, props.isStartDate)}
            onChange={e => props.onUpdate(DateService.dateFromDatepickerString(e.target.value))}
            min={DateService.minDate()}
            max={DateService.maxDate()}
        />
    );
}