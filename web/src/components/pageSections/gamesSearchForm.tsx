import * as React from 'react';
import { GameStatus } from '../../api/model';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import EnumDropdown from '../controls/enumDropdown';
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
            className="form"
        >
            <div className="formRow">
                <div className="formField">
                    <div className="formElement">
                        GameId
                    </div>
                    <div className="formElement">
                        <input
                            style={{width:"50px"}}
                            type={HtmlInputTypes.Number}
                            min={1}
                            value={emptyIfNull(query.gameId)}
                            onChange={e => onUpdate({ ...query, gameId: parseInt(e.target.value) })}
                        />
                    </div>
                </div>
                <div className="formField">
                    <div className="formElement">
                        Description
                    </div>
                    <div className="formElement">
                        <input
                            type={HtmlInputTypes.Text}
                            value={emptyIfNull(query.descriptionContains)}
                            onChange={e => onUpdate({ ...query, descriptionContains: nullIfEmpty(e.target.value) })}
                            autoFocus
                        />
                    </div>
                </div>
            </div>
            <div className="formRow">
                <div className="formField">
                    <div className="formElement">
                        Is public
                    </div>
                    <div className="formElement">
                        <TristateDropdown
                            name={"IsPublic"}
                            value={query.isPublic}
                            onChange={(_, value) => onUpdate({ ...query, isPublic: value })}
                        />
                    </div>
                </div>
                <div className="formField">
                    <div className="formElement">
                        Allow guests
                    </div>
                    <div className="formElement">
                        <TristateDropdown
                            name={"AllowGuests"}
                            value={query.allowGuests}
                            onChange={(_, value) => onUpdate({ ...query, allowGuests: value })}
                        />
                    </div>
                </div>
            </div>
            <div className="formRow">
                <div className="formField">
                    <div className="formElement">
                        Created by user
                    </div>
                    <div className="formElement">
                        <input
                            type={HtmlInputTypes.Text}
                            value={emptyIfNull(query.createdByUserName)}
                            onChange={e => onUpdate({ ...query, createdByUserName: nullIfEmpty(e.target.value) })}
                        />
                    </div>
                </div>
                <div className="formField">
                    <div className="formElement">
                        Contains user
                    </div>
                    <div className="formElement">
                        <input
                            type={HtmlInputTypes.Text}
                            value={emptyIfNull(query.playerUserName)}
                            onChange={e => onUpdate({ ...query, playerUserName: nullIfEmpty(e.target.value) })}
                        />
                    </div>
                </div>
            </div>
            <div className="formRow">
                <div className="formField">
                    <div className="formElement">
                        Status
                    </div>
                    <div className="formElement">
                        <EnumDropdown
                            name={"Status"}
                            value={query.statuses.length > 0 ? query.statuses[0] : null}
                            onChange={(_, value) => onUpdate({ ...query, statuses: [value] })}
                            enum={GameStatus}
                        />
                    </div>
                </div>
            </div>
            <div className="formRow">
                <div className="formField">
                    <div className="formElement">
                        Created between
                    </div>
                    <div className="formElement">
                        <DatePicker
                            value={query.createdAfter}
                            onUpdate={d => onUpdate({ ...query, createdAfter: d })}
                            isStartDate={true}
                        />
                    </div>
                    <div className="formElement">
                        and
                    </div>
                    <div className="formElement">
                        <DatePicker
                            value={query.createdBefore}
                            onUpdate={d => onUpdate({ ...query, createdBefore: d })}
                            isStartDate={false}
                        />
                    </div>
                </div>
            </div>
            <div className="formRow">
                <div className="formField">
                    <div className="formElement">
                        Last event between
                    </div>
                    <div className="formElement">
                        <DatePicker
                            value={query.lastEventAfter}
                            onUpdate={d => onUpdate({ ...query, lastEventAfter: d })}
                            isStartDate={true}
                        />
                    </div>
                    <div className="formElement">
                        and
                    </div>
                    <div className="formElement">
                        <DatePicker
                            value={query.lastEventBefore}
                            onUpdate={d => onUpdate({ ...query, lastEventBefore: d })}
                            isStartDate={false}
                        />
                    </div>
                </div>
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