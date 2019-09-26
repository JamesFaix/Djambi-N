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
            className="form"
        >
            <FormRow>
                <FormField label="ID">
                    <input
                        style={{width:"50px"}}
                        type={HtmlInputTypes.Number}
                        min={1}
                        value={emptyIfNull(query.gameId)}
                        onChange={e => onUpdate({ ...query, gameId: parseInt(e.target.value) })}
                    />
                </FormField>
                <FormField label="Description">
                    <input
                        type={HtmlInputTypes.Text}
                        value={emptyIfNull(query.descriptionContains)}
                        onChange={e => onUpdate({ ...query, descriptionContains: nullIfEmpty(e.target.value) })}
                        autoFocus
                    />
                </FormField>
            </FormRow>
            <FormRow>
                <FormField label="Is public">
                    <TristateDropdown
                        name={"IsPublic"}
                        value={query.isPublic}
                        onChange={(_, value) => onUpdate({ ...query, isPublic: value })}
                    />
                </FormField>
                <FormField label="Allow guests">
                    <TristateDropdown
                        name={"AllowGuests"}
                        value={query.allowGuests}
                        onChange={(_, value) => onUpdate({ ...query, allowGuests: value })}
                    />
                </FormField>
            </FormRow>
            <FormRow>
                <FormField label="Created by user">
                    <input
                        type={HtmlInputTypes.Text}
                        value={emptyIfNull(query.createdByUserName)}
                        onChange={e => onUpdate({ ...query, createdByUserName: nullIfEmpty(e.target.value) })}
                    />
                </FormField>
                <FormField label="Contains user">
                    <input
                        type={HtmlInputTypes.Text}
                        value={emptyIfNull(query.playerUserName)}
                        onChange={e => onUpdate({ ...query, playerUserName: nullIfEmpty(e.target.value) })}
                    />
                </FormField>
            </FormRow>
            <FormRow>
                <FormField label="Pending">
                    <input
                        type={HtmlInputTypes.CheckBox}
                        checked={query.statuses.includes(GameStatus.Pending)}
                        onChange={e => {
                            const s = GameStatus.Pending;
                            let statuses = [ ...query.statuses ];
                            if (e.target.checked) {
                                if (!statuses.includes(s)) {
                                    statuses.push(s);
                                }
                            } else {
                                statuses = statuses.filter(x => x !== s);
                            }
                            onUpdate({ ...query, statuses: statuses })
                        }}
                    />
                </FormField>
                <FormField label="In progress">
                    <input
                        type={HtmlInputTypes.CheckBox}
                        checked={query.statuses.includes(GameStatus.InProgress)}
                        onChange={e => {
                            const s = GameStatus.InProgress;
                            let statuses = [ ...query.statuses ];
                            if (e.target.checked) {
                                if (!statuses.includes(s)) {
                                    statuses.push(s);
                                }
                            } else {
                                statuses = statuses.filter(x => x !== s);
                            }
                            onUpdate({ ...query, statuses: statuses })
                        }}
                    />
                </FormField>
                <FormField label="Over">
                    <input
                        type={HtmlInputTypes.CheckBox}
                        checked={query.statuses.includes(GameStatus.Over)}
                        onChange={e => {
                            const s = GameStatus.Over;
                            let statuses = [ ...query.statuses ];
                            if (e.target.checked) {
                                if (!statuses.includes(s)) {
                                    statuses.push(s);
                                }
                            } else {
                                statuses = statuses.filter(x => x !== s);
                            }
                            onUpdate({ ...query, statuses: statuses })
                        }}
                    />
                </FormField>
                <FormField label="Canceled">
                    <input
                        type={HtmlInputTypes.CheckBox}
                        checked={query.statuses.includes(GameStatus.Canceled)}
                        onChange={e => {
                            const s = GameStatus.Canceled;
                            let statuses = [ ...query.statuses ];
                            if (e.target.checked) {
                                if (!statuses.includes(s)) {
                                    statuses.push(s);
                                }
                            } else {
                                statuses = statuses.filter(x => x !== s);
                            }
                            onUpdate({ ...query, statuses: statuses })
                        }}
                    />
                </FormField>
            </FormRow>
            <FormRow>
                <DateSpanField
                    label="Created"
                    startValue={query.createdAfter}
                    endValue={query.createdBefore}
                    onStartChange={d => onUpdate({ ...query, createdAfter: d })}
                    onEndChange={d => onUpdate({ ...query, createdBefore: d })}
                />
            </FormRow>
            <FormRow>
                <DateSpanField
                    label="Last event"
                    startValue={query.lastEventAfter}
                    endValue={query.lastEventBefore}
                    onStartChange={d => onUpdate({ ...query, lastEventAfter: d })}
                    onEndChange={d => onUpdate({ ...query, lastEventBefore: d })}
                />
            </FormRow>
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

const FormField : React.SFC<{
    label : string
}> = props => {
    return (
        <div className="formField">
            <div className="formElement">
                {props.label}
            </div>
            <div className="formElement">
                {props.children}
            </div>
        </div>
    );
}

const FormRow : React.SFC<{}> = props => {
    return (
        <div className="formRow">
            {props.children}
        </div>
    );
}

const DateSpanField : React.SFC<{
    label : string
    startValue : Date,
    endValue : Date,
    onStartChange : (d:Date) => void,
    onEndChange : (d:Date) => void
}> = props => {
    return (
        <div className="formField">
            <div className="formElement">
                {props.label} between
            </div>
            <div className="formElement">
                <DatePicker
                    value={props.startValue}
                    onUpdate={props.onStartChange}
                    isStartDate={true}
                />
            </div>
            <div className="formElement">
                and
            </div>
            <div className="formElement">
                <DatePicker
                    value={props.endValue}
                    onUpdate={props.onEndChange}
                    isStartDate={false}
                />
            </div>
        </div>
    );
}