const imagesDir = "../../resources/images";

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
        },
        images: {
            pieces: {
                assassin: `${imagesDir}/default/assassin.png`,
                chief: `${imagesDir}/default/chief.png`,
                corpse: `${imagesDir}/default/corpse.png`,
                diplomat: `${imagesDir}/default/diplomat.png`,
                gravedigger: `${imagesDir}/default/gravedigger.png`,
                reporter: `${imagesDir}/default/reporter.png`,
                thug: `${imagesDir}/default/thug.png`
            }
        },
        copy: {
            centerCellName: "Seat",
            pieces: {
                assassin: "Assassin",
                chief: "Chief",
                corpse: "Corpse",
                diplomat: "Diplomat",
                gravedigger: "Gravedigger",
                reporter: "Reporter",
                thug: "Thug"
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
        },
        images: {
            pieces: {
                assassin: `${imagesDir}/anesto/assassin.png`,
                chief: `${imagesDir}/anesto/chief.png`,
                corpse: `${imagesDir}/anesto/corpse.png`,
                diplomat: `${imagesDir}/anesto/diplomat.png`,
                gravedigger: `${imagesDir}/anesto/gravedigger.png`,
                reporter: `${imagesDir}/anesto/reporter.png`,
                thug: `${imagesDir}/anesto/thug.png`
            }
        },
        copy: {
            centerCellName: "Maze",
            pieces: {
                ...ThemeFactory.default.copy.pieces,
                diplomat: "Provocateur",
                gravedigger: "Necromobile",
                reporter: "Journalist",
                thug: "Militant"
            }
        }
    }

    public static readonly hotdogtown : Theme = {
        name: "Hotdogtown",
        colors: {
            ...ThemeFactory.default.colors,
        },
        images: {
            pieces: {
                assassin: `${imagesDir}/hotdogtown/assassin.png`,
                chief: `${imagesDir}/hotdogtown/chief.png`,
                corpse: `${imagesDir}/hotdogtown/corpse.png`,
                diplomat: `${imagesDir}/hotdogtown/diplomat.png`,
                gravedigger: `${imagesDir}/hotdogtown/gravedigger.png`,
                reporter: `${imagesDir}/hotdogtown/reporter.png`,
                thug: `${imagesDir}/hotdogtown/thug.png`
            }
        },
        copy: {
            centerCellName: "Booth",
            pieces: {
                assassin: "Fork",
                chief: "Ketchup",
                corpse: "Hotdog",
                diplomat: "Hugger",
                gravedigger: "Eater",
                reporter: "Fart",
                thug: "Fries"
            }
        }
    }

    public static readonly void : Theme = {
        name: "Void",
        colors: {
            ...ThemeFactory.default.colors,
            background: "#161616", //dark gray
            text: "#dcdcdc", //light gray
            border: "#dcdcdc",
            cells: {
                ...ThemeFactory.default.colors.cells,
                even: "#000000",
                odd: "#333333",
                center: "#EEEEEE"
            }
        },
        images: {
            pieces: {
                ...ThemeFactory.default.images.pieces,
                chief: `${imagesDir}/void/chief.png`,
                gravedigger: `${imagesDir}/void/gravedigger.png`,
                reporter: `${imagesDir}/void/reporter.png`,
            }
        },
        copy: {
            centerCellName: "Void",
            pieces: {
                assassin: "Hunter",
                chief: "Conduit",
                corpse: "Husk",
                diplomat: "Transporter",
                gravedigger: "Reaper",
                reporter: "Scientist",
                thug: "Zealot"
            }
        }
    }

    public static getThemes() : Map<string, Theme> {
        const themes = [
            ThemeFactory.default,
            ThemeFactory.anesto,
            ThemeFactory.hotdogtown,
            ThemeFactory.void
        ];

        return new Map<string, Theme>(themes.map(t => [t.name, t]));
    }
}