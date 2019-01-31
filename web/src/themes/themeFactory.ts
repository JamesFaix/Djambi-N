import Theme from "./theme";

export default class ThemeFactory {
    public static getDefaultTheme() : Theme {
        return {
            cellColorCenter : "#828282", //Gray
            cellColorEven : "#000000",   //Black
            cellColorOdd : "#FFFFFF",    //White

            cellHighlightSelectedColor : "#6AC921", //Green
            cellHighlightSelectedIntensity : 0.75,
            cellHighlightSelectionOptionColor : "#E5E500", //Yellow
            cellHighlightSelectionOptionIntensity : 0.5,

            centerCellName : "The Seat",

            pieceImageAssassin : "../resources/daggerEmoji.png",
            pieceImageChief : "../resources/crownEmoji.png",
            pieceImageCorpse : "../resources/skullEmoji.png",
            pieceImageDiplomat : "../resources/doveEmoji.png",
            pieceImageGravedigger : "../resources/pickEmoji.png",
            pieceImageReporter : "../resources/newspaperEmoji.png",
            pieceImageThug : "../resources/fistEmoji.png",

            pieceNameAssassin : "Assassin",
            pieceNameChief : "Chief",
            pieceNameCorpse : "Corpse",
            pieceNameDiplomat : "Diplomat",
            pieceNameGravedigger : "Gravedigger",
            pieceNameReporter : "Reporter",
            pieceNameThug : "Thug",

            playerColor0 : "#CC2B08", //Red
            playerColor1 : "#47CC08", //Green
            playerColor2 : "#08A9CC", //Blue
            playerColor3 : "#8D08CC", //Purple
            playerColor4 : "#CC8708", //Orange
            playerColor5 : "#CC0884", //Pink
            playerColor6 : "#08CC8B", //Teal
            playerColor7 : "#996A0C", //Brown

            selectionDescriptionSubject : "Pick up %s",
            selectionDescriptionMove : "Move to cell %s",
            selectionDescriptionMoveAndTarget : "Move to cell %s and target %s",
            selectionDescriptionTarget : "Target %s at cell %s",
            selectionDescriptionDrop : "Drop target piece at cell %s",
            selectionDescriptionVacate: "Vacate %s to cell %s",

            selectionPromptDrop : "Select a cell to drop the target piece in",
            selectionPromptMove : "Select a cell to move to",
            selectionPromptNull : "Click Commit or Reset",
            selectionPromptSubject : "Select a piece to move",
            selectionPromptTarget : "Select a piece to target",
            selectionPromptVacate : "Select a cell to vacate to",
        };
    }

    public static getHotDogTownTheme() : Theme {
        return {
            cellColorEven : null,
            cellColorCenter : null,
            cellColorOdd : null,

            cellHighlightSelectedColor : null,
            cellHighlightSelectedIntensity : null,
            cellHighlightSelectionOptionColor : null,
            cellHighlightSelectionOptionIntensity : null,

            centerCellName : "The Booth",

            pieceImageAssassin : null,
            pieceImageChief : null,
            pieceImageCorpse : null,
            pieceImageDiplomat : null,
            pieceImageGravedigger : null,
            pieceImageReporter : null,
            pieceImageThug: null,

            pieceNameAssassin : "Fork",
            pieceNameChief : "Sauce",
            pieceNameCorpse : "Hotdog",
            pieceNameDiplomat : "Hugger",
            pieceNameGravedigger : "Eater",
            pieceNameReporter : "Fart",
            pieceNameThug : "Fries",

            playerColor0 : null,
            playerColor1 : null,
            playerColor2 : null,
            playerColor3 : null,
            playerColor4 : null,
            playerColor5 : null,
            playerColor6 : null,
            playerColor7 : null,

            selectionDescriptionSubject : null,
            selectionDescriptionMove : null,
            selectionDescriptionMoveAndTarget : null,
            selectionDescriptionTarget : null,
            selectionDescriptionDrop : null,
            selectionDescriptionVacate: null,

            selectionPromptDrop : null,
            selectionPromptMove : null,
            selectionPromptNull : null,
            selectionPromptSubject : null,
            selectionPromptTarget : null,
            selectionPromptVacate : null,
        };
    }
}