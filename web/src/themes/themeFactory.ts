import Theme from './theme';

export default class ThemeFactory {
    private static getModernTheme() : Theme {
        return {
            cellStyle: {
                colorCenter: "#828282", //Gray
                colorEven: "#000000",   //Black
                colorOdd: "#FFFFFF",    //White
                borderColorCenter: null,
                borderColorEven: null,
                borderColorOdd: null,
            },
            cellHighlightStyle: {
                selectedColor: "#6AC921", //Green
                selectedIntensity: 0.75,
                selectionOptionColor: "#E5E500", //Yellow
                selectionOptionIntensity: 0.5,
            },
            centerCellName: "The Seat",
            pieces: {
                imageAssassin: "../resources/daggerEmoji.png",
                imageChief: "../resources/crownEmoji.png",
                imageCorpse: "../resources/skullEmoji.png",
                imageDiplomat: "../resources/doveEmoji.png",
                imageGravedigger: "../resources/pickEmoji.png",
                imageReporter: "../resources/newspaperEmoji.png",
                imageThug: "../resources/fistEmoji.png",

                nameAssassin: "Assassin",
                nameChief: "Chief",
                nameCorpse: "Corpse",
                nameDiplomat: "Diplomat",
                nameGravedigger: "Gravedigger",
                nameReporter: "Reporter",
                nameThug: "Thug",
            },
            players: {
                color0: "#CC2B08", //Red
                color1: "#47CC08", //Green
                color2: "#08A9CC", //Blue
                color3: "#8D08CC", //Purple
                color4: "#CC8708", //Orange
                color5: "#CC0884", //Pink
                color6: "#08CC8B", //Teal
                color7: "#996A0C", //Brown
            },
            gameCopy: {
                effectGameStatusChangedInProgress: "Game started.",
                effectGameStatusChangedOver: "Game over.",
                effectPieceAbandoned: "%(piece)s was abandoned.",
                effectPieceDropped: "%(piece)s was dropped at %(newCell)s.",
                effectPieceEnlisted: "%(piece)s was enlisted by player %(newPlayer)s.",
                effectPieceKilled: "%(piece)s was killed.",
                effectPieceMoved: "%(piece)s moved from %(oldCell)s to %(newCell)s.",
                effectPieceVacated: "%(piece)s fled %(center)s to %(newCell)s.",
                effectPlayerAdded: "%(player)s joined the game.",
                effectNeutralPlayerAdded: "Neutral player %(player)s added to the game.",
                effectPlayerOutOfMoves: "%(player)s is out of moves.",
                effectPlayerRemoved: "%(player)s was removed from the game.",
                effectPlayerStatusChangedAlive: "%(player)s will no longer accept a draw.",
                effectPlayerStatusChangedAcceptsDraw: "%(player)s will accept a draw.",
                effectPlayerStatusChangedConceded: "%(player)s conceded.",
                effectPlayerStatusChangedWillConcede: "%(player)s will concede at the start of their next turn.",
                effectPlayerStatusChangedEliminated: "%(player)s was eliminated.",
                effectPlayerStatusChangedVictorious: "%(player)s won.",
                effectTurnCycleAdvanced: "Turn cycle advanced by 1. [%(newCycle)s]",
                effectTurnCyclePlayerFellFromPower: "%(player)s fell from power. [%(newCycle)s]",
                effectTurnCyclePlayerRemoved: "%(player)s removed from the turn cycle. [%(newCycle)s]",
                effectTurnCyclePlayerRoseToPower: "%(player)s rose to power. [%(newCycle)s]",

                eventGameStarted: "Game started",
                eventTurnCommitted: "%(player)s took a turn",
                eventPlayerStatusChanged: "%(player)s changed their status",

                selectionDescriptionSubject: "Pick up %s",
                selectionDescriptionMove: "Move to cell %s",
                selectionDescriptionMoveAndTarget: "Move to cell %s and target %s",
                selectionDescriptionTarget: "Target %s at cell %s",
                selectionDescriptionDrop: "Drop target piece at cell %s",
                selectionDescriptionVacate: "Vacate %s to cell %s",

                selectionPromptDrop: "%s, select a cell to drop the target piece in.",
                selectionPromptMove: "%s, select a cell to move to.",
                selectionPromptSubject: "%s, select a piece to move.",
                selectionPromptTarget: "%s, select a piece to target.",
                selectionPromptVacate: "%s, select a cell to vacate to.",

                turnPromptCommit: "%s, end your turn or reset.",
                turnPromptDeadEnd: "%s, no further selections are available. You must reset."
            }
        };
    }

    private static getHotdogtownThemeFragment() : Theme {
        return {
            centerCellName: "The Booth",
            pieces: {
                nameAssassin: "Fork",
                nameChief: "Sauce",
                nameCorpse: "Hotdog",
                nameDiplomat: "Hugger",
                nameGravedigger: "Eater",
                nameReporter: "Fart",
                nameThug: "Fries",

                imageAssassin: "../resources/forkEmoji.png",
                imageChief: "../resources/ketchupEmoji.png",
                imageCorpse: "../resources/hotdogEmoji.png",
                imageDiplomat: "../resources/hugEmoji.png",
                imageGravedigger: "../resources/droolEmoji.png",
                imageReporter: "../resources/fartEmoji.png",
                imageThug: "../resources/friesEmoji.png",
            }
        };
    }

    private static getVoidThemeFragment() : Theme {
        return {
            centerCellName: "The Void",
            cellStyle: {
                colorCenter : "#EEEEEE", //Very light gray
                colorEven : "#000000",   //Black
                colorOdd : "#333333",    //Dark gray
                borderColorCenter: "#DDDDDD",
                borderColorEven: "#DDDDDD",
                borderColorOdd: "#DDDDDD"
            },
            pieces: {
                nameAssassin : "Hunter",
                nameChief : "Conduit",
                nameCorpse : "Husk",
                nameDiplomat : "Transporter",
                nameGravedigger : "Reaper",
                nameReporter : "Scientist",
                nameThug : "Zealot",

                imageAssassin : "../resources/daggerEmoji.png",
                imageChief : "../resources/eyeEmoji.png",
                imageCorpse : "../resources/skullEmoji.png",
                imageDiplomat : "../resources/doveEmoji.png",
                imageGravedigger : "../resources/sicleIcon.png",
                imageReporter : "../resources/testTubeEmoji.png",
                imageThug : "../resources/fistEmoji.png",
            }
        };
    }

    private static getClassicThemeFragment() : Theme {
        return {
            centerCellName : "The Maze",
            cellStyle: {
                colorCenter : "#000080", //Navy
                colorEven : "#a7a7a7",   //Medium gray
                colorOdd : "#a7a7a7",    //Medium gray
                borderColorCenter: "#FFFFFF",
                borderColorEven: "#000000",
                borderColorOdd: "#000000"
            },
            pieces: {
                nameAssassin : "Assassin",
                nameChief : "Chief",
                nameCorpse : "Corpse",
                nameDiplomat : "Provocateur",
                nameGravedigger : "Necromobile",
                nameReporter : "Journalist",
                nameThug : "Militant",

                imageAssassin : "../resources/classicAssassin.png",
                imageChief : "../resources/classicChief.png",
                imageCorpse : "../resources/classicCorpse.png",
                imageDiplomat : "../resources/classicDiplomat.png",
                imageGravedigger : "../resources/classicNecromobile.png",
                imageReporter : "../resources/classicReporter.png",
                imageThug : "../resources/classicMilitant.png",
            }
        };
    }

    private static merge(theme1 : Theme, theme2 : Theme) : Theme {
        return {...theme1, ...theme2};
    }

    public static get defaultTheme() : Theme {
        return this.getModernTheme();
    }

    public static get hotdogtownTheme() : Theme {
        return this.merge(this.getModernTheme(), this.getHotdogtownThemeFragment());
    }

    public static get voidTheme() : Theme {
        return this.merge(this.getModernTheme(), this.getVoidThemeFragment());
    }

    public static get classicTheme() : Theme {
        return this.merge(this.getModernTheme(), this.getClassicThemeFragment());
    }
}