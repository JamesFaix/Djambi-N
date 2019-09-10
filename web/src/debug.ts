export interface DebugSettings {
    showBoardTooltips : boolean,
    showCellAndPieceIds : boolean,
    logApi : boolean,
    logSse : boolean,
    logRedux : boolean
}

export const defaultDebugSettings : DebugSettings = {
    showBoardTooltips : false,
    showCellAndPieceIds : false,
    logApi : false,
    logSse : false,
    logRedux : false
}