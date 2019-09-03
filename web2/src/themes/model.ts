interface Theme {
    name : string,
    colors : ThemeColors
}

interface ThemeColors {
    background : string,
    text : string,
    border : string,

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
    border: string,
    boardBorder: string,

    selectedColor: string,
    selectedIntensity: number,
    selectableColor: string,
    selectableIntensity: number
}