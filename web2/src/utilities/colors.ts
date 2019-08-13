export default class Colors {
    static getColorFromPlayerColorId(colorId : number) : string {
        switch (colorId) {
            case 0: return "blue";
            case 1: return "red";
            case 2: return "green";
            case 3: return "orange";
            case 4: return "brown";
            case 5: return "teal";
            case 6: return "magenta";
            case 7: return "gold";
            default:
                throw "Unsupported colorID";
        }
    }
}