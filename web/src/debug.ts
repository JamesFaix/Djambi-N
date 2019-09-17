export interface DebugSettings {
    showBoardTooltips : boolean,
    showCellAndPieceIds : boolean,
    logApi : boolean,
    logSse : boolean,
    logRedux : boolean,
    showNotificationsSeconds : number
}

export const defaultDebugSettings : DebugSettings = {
    showBoardTooltips : true,
    showCellAndPieceIds : false,
    logApi : false,
    logSse : false,
    logRedux : false,
    showNotificationsSeconds : 5
}