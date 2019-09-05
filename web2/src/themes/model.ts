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

    player0 : string,
    player1 : string,
    player2 : string,
    player3 : string,
    player4 : string,
    player5 : string,
    player6 : string,
    player7 : string

    cells: ThemeCellColors
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
    selectedIntensity: number,
    selectableColor: string,
    selectableIntensity: number
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