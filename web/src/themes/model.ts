export interface Theme {
    name : string,
    colors : ThemeColors,
    images : ThemeImagePaths,
    copy : ThemeCopy,
    fonts : ThemeFonts
}

interface ThemeColors {
    background : string,
    text : string,
    headerText : string,
    border : string,
    altRowText : string,
    altRowBackground : string,

    hoverText : string,
    hoverBackground : string,

    positiveButtonBackground : string,
    negativeButtonBackground : string,

    cells: ThemeCellColors,
    players: ThemePlayerColors
}

interface ThemeCellColors {
    even: string,
    odd: string,
    center: string,

    evenBorder : string,
    oddBorder: string,
    centerBorder : string,

    evenText : string,
    oddText : string,
    centerText : string,

    boardBorder: string,

    selectedColor: string,
    selectableColor: string,
}

interface ThemePlayerColors {
    p0 : string,
    p1 : string,
    p2 : string,
    p3 : string,
    p4 : string,
    p5 : string,
    p6 : string,
    p7 : string,
    neutral : string,
    placeholder : string //Color that is replaced on sprites by player colors
}

interface ThemeImagePaths {
    pieces : ThemePieceImagePaths
}

interface ThemePieceImagePaths {
    assassin : string,
    chief : string,
    corpse : string,
    diplomat : string,
    gravedigger : string,
    reporter : string,
    thug : string
}

interface ThemeCopy {
    centerCellName : string,
    gameTitle : string,
    pieces : ThemePieceNames
}

interface ThemePieceNames {
    assassin : string,
    chief : string,
    corpse : string,
    diplomat : string,
    gravedigger : string,
    reporter : string,
    thug : string
}

interface ThemeFonts {
    headerFamily : string,
    normalFamily : string
}