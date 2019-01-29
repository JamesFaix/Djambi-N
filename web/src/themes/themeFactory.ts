import Theme from "./theme";

export default class ThemeFactory {
    public static getDefaultTheme() : Theme {
        return {
            cellColorBlack : "#000000",
            cellColorCenter : "#828282",
            cellColorWhite : "#FFFFFF",

            cellHighlightSelectedColor : "#6AC921", //Green
            cellHighlightSelectedIntensity : 0.75,
            cellHighlightSelectionOptionColor : "#E5E500", //Yellow
            cellHighlightSelectionOptionIntensity : 0.5,

            centerCellName : "The Seat",

            pieceEmojiAssassin : "&#x1F5E1",
            pieceEmojiChief : "&#x1F451",
            pieceEmojiCorpse : "&#x1F480",
            pieceEmojiDiplomat : "&#x1F54A",
            pieceEmojiGravedigger : "&#x26CF",
            pieceEmojiReporter : "&#x1F4F0",
            pieceEmojiThug: "&#x270A",

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

            selectionDescriptionSubject : "Move %s",
            selectionDescriptionMove : " to cell %s",
            selectionDescriptionMoveAndTarget : " to cell %s and target %s",
            selectionDescriptionTarget : " and target %s at cell %s",
            selectionDescriptionDrop : ", then drop target piece at cell %s",
            selectionDescriptionVacate: ", finally vacate %s to cell %s",

            selectionPromptDrop : "Select a cell to drop the target piece in",
            selectionPromptMove : "Select a cell to move to",
            selectionPromptNull : "(Click Done or Reset)",
            selectionPromptSubject : "Select a piece to move",
            selectionPromptTarget : "Select a piece to target",
            selectionPromptVacate : "Select a cell to vacate to",
        };
    }

    public static getHotDogTownTheme() : Theme {
        return {
            cellColorBlack : null,
            cellColorCenter : null,
            cellColorWhite : null,

            cellHighlightSelectedColor : null,
            cellHighlightSelectedIntensity : null,
            cellHighlightSelectionOptionColor : null,
            cellHighlightSelectionOptionIntensity : null,

            centerCellName : "The Booth",

            pieceEmojiAssassin : "&#x1F374",
            pieceEmojiChief : "&#x1F96B",
            pieceEmojiCorpse : "&#x1F32D",
            pieceEmojiDiplomat : "&#x1F917",
            pieceEmojiGravedigger : "&#x1F924",
            pieceEmojiReporter : "&#x1F4A8",
            pieceEmojiThug: "&#x1F35F",

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