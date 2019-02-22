import moment = require("moment");

export default class DateService {

    public format(date : Date) : string {
        return moment
            .utc(date)
            .local()
            .format('MM/DD/YY hh:mm a');
    }
}