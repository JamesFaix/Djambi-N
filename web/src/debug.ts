export default class Debug {
    static Initialize() {
        const w = (window as any);
        w.getDebugSetting = (name : string) => this.getSetting(name);
        w.setDebugSetting = (name : string, value : any) => this.setSetting(name, value);
    }

    private static getSetting(name : string) : any {
        return (this as any)[name];
    }

    private static setSetting(name : string, value : any) : void {
        (this as any)[name] = value;
    }

    public static readonly showPieceAndCellIds = false;

    public static readonly showCellLabels = false;

    public static readonly logApiErrors = true;

    public static readonly logApiSuccesses = false;
}