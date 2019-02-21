import { Location } from './api/model';
import * as Sprintf from 'sprintf-js';

export default class CopyService {
    public static locationToString(location : Location) : string {
        if (location.x === 0 && location.y === 0 && location.region === 0) {
            return "(0)";
        }

        return Sprintf.sprintf("(%i, %i, %i)", location.region, location.x, location.y);
    }
}