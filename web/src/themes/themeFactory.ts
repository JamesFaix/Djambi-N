import { Theme } from "./model";

const imagesDir = "../../resources/images";

const whiteHex = "#FFFFFF";
const blackHex = "#000000";
const medGrayHex = "#828282";
const darkGrayHex = "#161616";
const lightGrayHex = "#DCDCDC";

export default class ThemeFactory {
    public static readonly default : Theme = {
        name: "Default",
        colors: {
            background: darkGrayHex,
            text: lightGrayHex,
            headerText: lightGrayHex,
            border: lightGrayHex,
            hoverBackground: lightGrayHex,
            hoverText: darkGrayHex,
            disabled: "#505050",
            altRowBackground: "#333333",
            altRowText: lightGrayHex,
            positiveButtonBackground: "mediumseagreen",
            negativeButtonBackground: "indianred",

            cells: {
                even: blackHex,
                odd: "#333333",
                center: "#EEEEEE",

                centerBorder: "#DDDDDD",
                evenBorder: "#DDDDDD",
                oddBorder: "#DDDDDD",

                evenText: "black",
                oddText: "white",
                centerText: "white",

                boardBorder: "black",

                selectedColor: "#6AC921", //Green
                selectableColor: "#E5E500", //Yellow
            },

            players: {
                p0: "blue",
                p1: "red",
                p2: "green",
                p3: "orange",
                p4: "brown",
                p5: "teal",
                p6: "magenta",
                p7: "gold",
                placeholder: "#CCCC33",
                neutral: "#555555"
            },

            infoBackground: "palegoldenrod",
            errorBackground: "lightcoral"
        },
        images: {
            pieces: {
                hunter: `${imagesDir}/default/hunter.png`,
                conduit: `${imagesDir}/default/conduit.png`,
                corpse: `${imagesDir}/default/corpse.png`,
                diplomat: `${imagesDir}/default/diplomat.png`,
                reaper: `${imagesDir}/default/reaper.png`,
                scientist: `${imagesDir}/default/scientist.png`,
                thug: `${imagesDir}/default/thug.png`
            }
        },
        copy: {
            gameTitle: "Apex",
            centerCellName: "Apex",
            pieces: {
                hunter: "Hunter",
                conduit: "Conduit",
                corpse: "Corpse",
                diplomat: "Diplomat",
                reaper: "Reaper",
                scientist: "Scientist",
                thug: "Thug"
            }
        },
        fonts: {
            titleFamily: "Century Gothic, Geneva, sans-serif",
            headerFamily: "Century Gothic, Geneva, sans-serif",
            normalFamily: "Century Gothic, Geneva, sans-serif"
        }
    };

    public static readonly chess : Theme = {
        ...ThemeFactory.default,
        name: "Chess",
        colors: {
            ...ThemeFactory.default.colors,
            background: "white",
            text: "black",
            headerText: "black",
            border: "gainsboro",
            hoverText: "white",
            hoverBackground: "black",
            disabled: "gainsboro",
            altRowText: "black",
            altRowBackground: "gainsboro",

            cells: {
                ...ThemeFactory.default.colors.cells,
                even: "white",
                odd: "black",
                center: medGrayHex,

                evenBorder: null,
                oddBorder: null,
                centerBorder: null,
            },
        }
    }

    public static readonly anesto : Theme = {
        name: "Anesto",
        colors: {
            ...ThemeFactory.default.colors,
            background: "oldlace",
            text: "black",
            headerText: "black",
            border: "gainsboro",
            hoverText: "white",
            hoverBackground: "black",
            disabled: "gainsboro",
            altRowText: "black",
            altRowBackground: "gainsboro",
            cells: {
                ...ThemeFactory.default.colors.cells,
                //Cell colors must be in hex
                even: medGrayHex,
                odd: medGrayHex,
                center: "#000080", //Navy

                evenBorder: blackHex,
                oddBorder: blackHex,
                centerBorder: whiteHex,

                evenText: blackHex,
                oddText: blackHex,
                centerText: whiteHex,
            }
        },
        images: {
            pieces: {
                hunter: `${imagesDir}/anesto/hunter.png`,
                conduit: `${imagesDir}/anesto/conduit.png`,
                corpse: `${imagesDir}/anesto/corpse.png`,
                diplomat: `${imagesDir}/anesto/diplomat.png`,
                reaper: `${imagesDir}/anesto/reaper.png`,
                scientist: `${imagesDir}/anesto/scientist.png`,
                thug: `${imagesDir}/anesto/thug.png`
            }
        },
        copy: {
            gameTitle: "Djambi",
            centerCellName: "Maze",
            pieces: {
                ...ThemeFactory.default.copy.pieces,
                diplomat: "Provocateur",
                reaper: "Necromobile",
                scientist: "Journalist",
                thug: "Militant"
            }
        },
        fonts: {
            titleFamily: "Georgia, serif",
            headerFamily: "Georgia, serif",
            normalFamily: "Georgia, serif"
        }
    }

    public static readonly hotdogtown : Theme = {
        name: "Hotdogtown",
        colors: {
            ...ThemeFactory.default.colors,
            background: "lemonchiffon",
            text: "black",
            headerText: "indianred",
            border: "gainsboro",
            hoverText: "white",
            hoverBackground: "black",
            disabled: "gainsboro",
            altRowText: "black",
            altRowBackground: "gainsboro",
            cells: {
                ...ThemeFactory.default.colors.cells,
                odd: "#00FFFF", //cyan
                even: whiteHex,
            }
        },
        images: {
            pieces: {
                hunter: `${imagesDir}/hotdogtown/hunter.png`,
                conduit: `${imagesDir}/hotdogtown/conduit.png`,
                corpse: `${imagesDir}/hotdogtown/corpse.png`,
                diplomat: `${imagesDir}/hotdogtown/diplomat.png`,
                reaper: `${imagesDir}/hotdogtown/reaper.png`,
                scientist: `${imagesDir}/hotdogtown/scientist.png`,
                thug: `${imagesDir}/hotdogtown/thug.png`
            }
        },
        copy: {
            gameTitle: "Hotdogtown",
            centerCellName: "Booth",
            pieces: {
                hunter: "Fork",
                conduit: "Ketchup",
                corpse: "Hotdog",
                diplomat: "Hugger",
                reaper: "Eater",
                scientist: "Fart",
                thug: "Fries"
            }
        },
        fonts: {
            ...ThemeFactory.default.fonts,
            titleFamily: "Comic Sans MS",
            headerFamily: "Comic Sans MS",
        }
    }

    public static getThemes() : Map<string, Theme> {
        const themes = [
            ThemeFactory.default,
            ThemeFactory.chess,
            ThemeFactory.anesto,
            ThemeFactory.hotdogtown
        ];

        return new Map<string, Theme>(themes.map(t => [t.name, t]));
    }
}