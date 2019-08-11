export function boolToYesOrNo(value : boolean) : string {
    if (value === true) {
        return "Yes";
    } else if (value === false) {
        return "No"
    } else { //null/undefined
        return null;
    }
}