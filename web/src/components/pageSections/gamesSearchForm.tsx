import * as React from 'react';
import { GameStatus } from '../../api/model';
import { State as AppState } from '../../store/root';
import { useSelector } from 'react-redux';
import TristateDropdown from '../controls/tristateDropdown';
import { IconSubmitButton } from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';
import DateService from '../../utilities/dates';
import { NumberInput, TextInput, Checkbox, DatePicker } from '../controls/input';

const GamesSearchForm : React.SFC<{}> = _ => {
    const query = useSelector((state : AppState) => state.search.query);
    const onUpdate = Controller.Forms.updateGamesQuery;

    return (
        <form
            onSubmit={e => {
                e.preventDefault();
                Controller.Search.searchGames(query):
            }}
            className="form"
        >
            <FormRow>
                <FormField label="ID">
                    <NumberInput
                        value={query.gameId}
                        onChange={x => onUpdate({ ...query, gameId: x })}
                        min={1}
                        style={{width:"50px"}}
                    />
                </FormField>
                <FormField label="Description">
                    <TextInput
                        value={emptyIfNull(query.descriptionContains)}
                        onChange={x => onUpdate({ ...query, descriptionContains: nullIfEmpty(x) })}
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
                    <TextInput
                        value={emptyIfNull(query.createdByUserName)}
                        onChange={x => onUpdate({ ...query, createdByUserName: nullIfEmpty(x) })}
                    />
                </FormField>
                <FormField label="Contains user">
                    <TextInput
                        value={emptyIfNull(query.playerUserName)}
                        onChange={x => onUpdate({ ...query, playerUserName: nullIfEmpty(x) })}
                    />
                </FormField>
            </FormRow>
            <FormRow>
                <FormField label="Pending">
                    <Checkbox
                        value={query.statuses.includes(GameStatus.Pending)}
                        onChange={x => onUpdate({ ...query, statuses: getStatuses(x, GameStatus.Pending, query.statuses)})}
                    />
                </FormField>
                <FormField label="In progress">
                    <Checkbox
                        value={query.statuses.includes(GameStatus.InProgress)}
                        onChange={x => onUpdate({ ...query, statuses: getStatuses(x, GameStatus.InProgress, query.statuses)})}
                    />
                </FormField>
                <FormField label="Over">
                    <Checkbox
                        value={query.statuses.includes(GameStatus.Over)}
                        onChange={x => onUpdate({ ...query, statuses: getStatuses(x, GameStatus.Over, query.statuses)})}
                    />
                </FormField>
                <FormField label="Canceled">
                    <Checkbox
                        value={query.statuses.includes(GameStatus.Canceled)}
                        onChange={x => onUpdate({ ...query, statuses: getStatuses(x, GameStatus.Canceled, query.statuses)})}
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

function getStatuses(checked : boolean, affectedStatus : GameStatus, currentStatuses : GameStatus[])
{
    if (checked) {
        if (currentStatuses.includes(affectedStatus)) {
            return currentStatuses;
        } else {
            return currentStatuses.concat([affectedStatus]);
        }
    } else {
        return currentStatuses.filter(x => x !== affectedStatus);
    }
}

function emptyIfNull(value : any) : string {
    return value === null ? "" : value;
}

function nullIfEmpty(value : string) : string {
    return value === "" ? null : value;
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
                    value={props.startValue ? props.startValue : DateService.minDate()}
                    onChange={props.onStartChange}
                    min={DateService.minDate()}
                    max={DateService.maxDate()}
                />
            </div>
            <div className="formElement">
                and
            </div>
            <div className="formElement">
                <DatePicker
                    value={props.endValue ? props.endValue : DateService.maxDate()}
                    onChange={props.onEndChange}
                    min={DateService.minDate()}
                    max={DateService.maxDate()}
                />
            </div>
        </div>
    );
}