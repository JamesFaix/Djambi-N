import * as React from 'react';
import DateService from '../../utilities/dates';

export enum HtmlInputTypes {
    Text = "text",
    CheckBox = "checkbox",
    Number = "number",
    Password = "password",
    Range = "range",
    Date = "date"
    //Fill out others as needed https://www.w3schools.com/html/html_form_input_types.asp
}

function emptyIfNull (x : any) : any {
    return x === null ? "" : x;
}

export const Checkbox : React.SFC<{
    value ?: boolean,
    onChange ?: (value : boolean) => void,
    style ?: React.CSSProperties,
    autoFocus ?: boolean
}> = props => {
    return (
        <input
            type={HtmlInputTypes.CheckBox}
            checked={props.value}
            onChange={e => props.onChange(e.target.checked)}
            style={props.style}
            autoFocus={props.autoFocus}
        />
    );
}

export const TextInput : React.SFC<{
    value ?: string,
    onChange ?: (value : string) => void
    style ?: React.CSSProperties,
    autoFocus ?: boolean,
    autoComplete ?: string
}> = props => {
    return (
        <input
            type={HtmlInputTypes.Text}
            value={emptyIfNull(props.value)}
            onChange={e => props.onChange(e.target.value)}
            style={props.style}
            autoFocus={props.autoFocus}
            autoComplete={props.autoComplete}
        />
    )
}

export const NumberInput : React.SFC<{
    value ?: number,
    onChange ?: (value : number) => void,
    min ?: number,
    max ?: number
    style ?: React.CSSProperties,
    autoFocus ?: boolean
}> = props => {
    return (
        <input
            type={HtmlInputTypes.Number}
            value={emptyIfNull(props.value)}
            onChange={e => props.onChange(Number(e.target.value))}
            min={props.min}
            max={props.max}
            style={props.style}
            autoFocus={props.autoFocus}
        />
    )
}

export const DatePicker : React.SFC<{
    value ?: Date,
    onChange ?: (value : Date) => void,
    min ?: Date,
    max ?: Date,
    style ?: React.CSSProperties,
    autoFocus ?: boolean
}> = props => {
    return (
        <input
            type={HtmlInputTypes.Date}
            value={DateService.dateToDatepickerString(props.value)}
            onChange={e => props.onChange(DateService.dateFromDatepickerString(e.target.value))}
            min={props.min ? DateService.dateToDatepickerString(props.min) : undefined}
            max={props.max ? DateService.dateToDatepickerString(props.max) : undefined}
            style={props.style}
            autoFocus={props.autoFocus}
        />
    )
}