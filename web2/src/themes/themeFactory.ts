export default class ThemeFactory {
    public static readonly default : Theme = {
        name: "Default",
        colors: {
            background: "white",
            text: "black",
            border: "gainsboro",
            hoverText: "white",
            hoverBackground: "black",

            player0: "blue",
            player1: "red",
            player2: "green",
            player3: "orange",
            player4: "brown",
            player5: "teal",
            player6: "magenta",
            player7: "gold",

            cells: {
                //Cell colors must be in hex
                even: "#FFFFFF",
                odd: "#000000",
                center: "#828282", //Medium gray
                border: null,
                boardBorder: "black",

                selectedColor: "#6AC921", //Green
                selectedIntensity: 0.75,
                selectableColor: "#E5E500", //Yellow
                selectableIntensity: 0.5
            }
        }
    };

    public static readonly anesto : Theme = {
        name: "Anesto",
        colors: {
            ...ThemeFactory.default.colors,
            cells: {
                ...ThemeFactory.default.colors.cells,
                //Cell colors must be in hex
                even: "#828282", //Gray
                odd: "#828282", //Gray
                center: "#000080", //Navy
                border: "black"
            }
        }
    }

    public static getThemes() : Map<string, Theme> {
        const themes = [
            ThemeFactory.default,
            ThemeFactory.anesto
        ];

        return new Map<string, Theme>(themes.map(t => [t.name, t]));
    }
}