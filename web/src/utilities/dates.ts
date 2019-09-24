import Moment from 'moment';

export function dateToString(d : Date) : string {
    return Moment
        .utc(d)
        .local()
        .format("M/D/YY h:mma");
}

export function dateFromDatepickerString(str : string) : Date {
    const datetime = `${str}T00:00:00.000`;
    const ms = Moment(datetime).utc().valueOf();
    const d = new Date(ms);
    console.log(`dateFromString(${str}) is ${d}`)
    return d;
}

export function dateToDatepickerString(d : Date, isStartDate : boolean) : string {
    if (!d) {
        return isStartDate ? DateService.minDate() : DateService.maxDate();
    }

    return Moment
        .utc(d)
        .local()
        .format("YYYY-MM-DD");
}

export class DateService {
    public static minDate() {
        return "2000-01-01";
    }

    public static maxDate() {
        return Moment().add(1, "days").format("YYYY-MM-DD");
    }
}