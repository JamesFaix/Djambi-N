export default class Debug {
    static init() {
        const w = (window as any);
        w.debugGet = (name : string) => this.getSetting(name);
        w.debugSet = (name : string, value : any) => this.setSetting(name, value);
    }

    private static getSetting(name : string) : any {
        return (this as any)[name];
    }

    private static setSetting(name : string, value : any) : void {
        (this as any)[name] = value;
    }

    public static readonly showPieceAndCellIds = false;

    public static readonly showCellLabels = false;

    public static readonly logApi = false;

    public static readonly logRedux = false;

    public static readonly logSse = false;
}