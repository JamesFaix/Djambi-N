import Moment from 'moment';

export function dateToString(d : Date) {
    return Moment
        .utc(d)
        .local()
        .format("M/D/YY h:mma");
}