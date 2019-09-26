import Moment from 'moment';

export default class DateService {
    public static dateToString(d : Date) : string {
        return Moment
            .utc(d)
            .local()
            .format("M/D/YY h:mma");
    }

    public static minDate() {
        return new Date(2000, 1, 1);
    }

    public static maxDate() {
        return Moment().add(1, "days").toDate();
    }
}