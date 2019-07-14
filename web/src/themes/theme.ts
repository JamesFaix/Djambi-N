/*
 * The nested objects inside Theme are intentionally only 1 layer deep.
 *
 * This is so two themes can be combined, with one overriding parts of the other,
 * pretty simply using {...x1, ...x2} syntax.
 *
 * Each nested object is optional, but inside each object the properties are not optional.
 * This is because if a theme is changing part of that module,
 * it should probably update all of it to match.
 */

export interface ThemePageStyle {
    backgroundColor : string,
    textColor : string,
    borderColor : string,
    hintTextColor : string
}

export interface ThemeCellStyle {
    borderColorCenter : string,
    borderColorEven : string,
    borderColorOdd : string,
    colorCenter : string,
    colorEven : string,
    colorOdd : string,
    textColorCenter : string,
    textColorEven : string,
    textColorOdd : string
}

export interface ThemeCellHighlightStyle {
    selectedColor : string,
    selectedIntensity : number,
    selectionOptionColor : string,
    selectionOptionIntensity : number
}

export interface ThemeGameCopy {
    effectGameStatusChangedInProgress : string,
    effectGameStatusChangedOver : string,
    effectNeutralPlayerAdded : string,
    effectPieceAbandoned : string,
    effectPieceDropped : string,
    effectPieceEnlisted : string,
    effectPieceKilled : string,
    effectPieceMoved : string,
    effectPieceVacated : string,
    effectPlayerAdded : string,
    effectPlayerOutOfMoves : string,
    effectPlayerRemoved : string,
    effectPlayerStatusChangedAlive : string,
    effectPlayerStatusChangedAcceptsDraw : string,
    effectPlayerStatusChangedConceded : string,
    effectPlayerStatusChangedWillConcede : string,
    effectPlayerStatusChangedEliminated : string,
    effectPlayerStatusChangedVictorious : string,
    effectTurnCycleAdvanced : string,
    effectTurnCyclePlayerFellFromPower : string,
    effectTurnCyclePlayerRemoved : string,
    effectTurnCyclePlayerRoseToPower : string,

    eventGameStarted : string,
    eventTurnCommitted : string,
    eventPlayerStatusChanged : string,

    selectionDescriptionDrop : string,
    selectionDescriptionMove : string,
    selectionDescriptionMoveAndTarget : string,
    selectionDescriptionSubject : string,
    selectionDescriptionTarget : string,
    selectionDescriptionVacate : string,

    selectionPromptDrop : string,
    selectionPromptMove : string,
    selectionPromptSubject : string,
    selectionPromptTarget : string,
    selectionPromptVacate : string,

    turnPromptCommit : string,
    turnPromptDeadEnd : string
}

export interface ThemePieces {
    imageAssassin : string,
    imageChief : string,
    imageCorpse : string,
    imageDiplomat : string,
    imageGravedigger : string,
    imageReporter : string,
    imageThug : string,

    nameAssassin : string,
    nameChief : string,
    nameCorpse : string,
    nameDiplomat : string,
    nameGravedigger : string,
    nameReporter : string,
    nameThug : string
}

export interface ThemePlayers {
    color0 : string,
    color1 : string,
    color2 : string,
    color3 : string,
    color4 : string,
    color5 : string,
    color6 : string,
    color7 : string
}

export default interface Theme {
    pageStyle ? : ThemePageStyle,
    cellStyle ? : ThemeCellStyle,
    cellHighlightStyle ? : ThemeCellHighlightStyle,
    gameCopy ? : ThemeGameCopy,
    pieces ? : ThemePieces,
    players ? : ThemePlayers,
    centerCellName ? : string
};;;;;;;;;;