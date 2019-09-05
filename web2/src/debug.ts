export interface DebugSettings {
    showCellLabels : boolean,
    showCellAndPieceIds : boolean,
    logApi : boolean,
    logSse : boolean,
    logRedux : boolean
}

export const defaultDebugSettings : DebugSettings = {
    showCellLabels : false,
    showCellAndPieceIds : false,
    logApi : false,
    logSse : false,
    logRedux : false
}