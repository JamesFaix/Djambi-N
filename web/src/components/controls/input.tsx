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

interface InputBaseProps<T> {
    value ?: T,
    onChange ?: (value:T) => void,
    style ?: React.CSSProperties,
    autoFocus ?: boolean
}

interface CheckBoxProps extends InputBaseProps<boolean> {}

export const Checkbox : React.SFC<CheckBoxProps> = props => {
    return (
        <input
            type={HtmlInputTypes.CheckBox}
            checked={props.value}
            onChange={e => props.onChange ? props.onChange(e.target.checked) : null}
            style={props.style}
            autoFocus={props.autoFocus}
        />
    );
}

interface TextInputProps extends InputBaseProps<string> {
    autoComplete ?: string
}

export const TextInput : React.SFC<TextInputProps> = props => {
    return (
        <input
            type={HtmlInputTypes.Text}
            value={emptyIfNull(props.value)}
            onChange={e => props.onChange ? props.onChange(e.target.value) : null}
            style={props.style}
            autoFocus={props.autoFocus}
            autoComplete={props.autoComplete}
        />
    )
}

interface NumberInputProps extends InputBaseProps<number> {
    min ?: number,
    max ?: number
}

export const NumberInput : React.SFC<NumberInputProps> = props => {
    return (
        <input
            type={HtmlInputTypes.Number}
            value={emptyIfNull(props.value)}
            onChange={e => props.onChange ? props.onChange(Number(e.target.value)) : null}
            min={props.min}
            max={props.max}
            style={props.style}
            autoFocus={props.autoFocus}
        />
    )
}

interface DatePickerProps extends InputBaseProps<Date> {
    min ?: Date,
    max ?: Date
}

export const DatePicker : React.SFC<DatePickerProps> = props => {
    return (
        <input
            type={HtmlInputTypes.Date}
            value={DateService.dateToDatepickerString(props.value)}
            onChange={e => props.onChange ? props.onChange(DateService.dateFromDatepickerString(e.target.value)) : null}
            min={props.min ? DateService.dateToDatepickerString(props.min) : undefined}
            max={props.max ? DateService.dateToDatepickerString(props.max) : undefined}
            style={props.style}
            autoFocus={props.autoFocus}
        />
    )
}