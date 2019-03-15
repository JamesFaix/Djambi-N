import Theme from './theme';

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

            effectMessageGameStatusChangedInProgress : "Game started.",
            effectMessageGameStatusChangedOver : "Game over.",
            effectMessagePieceAbandoned : "%(piece)s was abandoned.",
            effectMessagePieceDropped : "%(piece)s was dropped at %(newCell)s.",
            effectMessagePieceEnlisted : "%(piece)s was enlisted by player %(newPlayer)s.",
            effectMessagePieceKilled : "%(piece)s was killed.",
            effectMessagePieceMoved : "%(piece)s moved from %(oldCell)s to %(newCell)s.",
            effectMessagePieceVacated : "%(piece)s fled %(center)s to %(newCell)s.",
            effectMessagePlayerAdded : "%(player)s joined the game.",
            effectMessageNeutralPlayerAdded : "Neutral player %(player)s added to the game.",
            effectMessagePlayerOutOfMoves : "%(player)s is out of moves.",
            effectMessagePlayerRemoved : "%(player)s was removed from the game.",
            effectMessagePlayerStatusChangedAlive : "%(player)s will no longer accept a draw.",
            effectMessagePlayerStatusChangedAcceptsDraw : "%(player)s will accept a draw.",
            effectMessagePlayerStatusChangedConceded : "%(player)s conceded.",
            effectMessagePlayerStatusChangedWillConcede : "%(player)s will concede at the start of their next turn.",
            effectMessagePlayerStatusChangedEliminated : "%(player)s was eliminated.",
            effectMessagePlayerStatusChangedVictorious : "%(player)s won.",
            effectMessageTurnCycleAdvanced : "Turn cycle advanced by 1. [%(newCycle)s]",
            effectMessageTurnCyclePlayerFellFromPower : "%(player)s fell from power. [%(newCycle)s]",
            effectMessageTurnCyclePlayerRemoved : "%(player)s removed from the turn cycle. [%(newCycle)s]",
            effectMessageTurnCyclePlayerRoseToPower : "%(player)s rose to power. [%(newCycle)s]",

            eventMessageGameStarted : "Game started",
            eventMessageTurnCommitted : "%(player)s took a turn",
            eventMessagePlayerStatusChanged : "%(player)s changed their status",

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

            selectionPromptDrop : "%s, select a cell to drop the target piece in.",
            selectionPromptMove : "%s, select a cell to move to.",
            selectionPromptSubject : "%s, select a piece to move.",
            selectionPromptTarget : "%s, select a piece to target.",
            selectionPromptVacate : "%s, select a cell to vacate to.",

            turnPromptCommit: "%s, end your turn or reset.",
            turnPromptDeadEnd: "%s, no further selections are available. You must reset."
        };
    }

    public static getHotDogTownTheme() : Theme {
        return {
            centerCellName : "The Booth",

            pieceNameAssassin : "Fork",
            pieceNameChief : "Sauce",
            pieceNameCorpse : "Hotdog",
            pieceNameDiplomat : "Hugger",
            pieceNameGravedigger : "Eater",
            pieceNameReporter : "Fart",
            pieceNameThug : "Fries",

            pieceImageAssassin : "../resources/forkEmoji.png",
            pieceImageChief : "../resources/ketchupEmoji.png",
            pieceImageCorpse : "../resources/hotdogEmoji.png",
            pieceImageDiplomat : "../resources/hugEmoji.png",
            pieceImageGravedigger : "../resources/droolEmoji.png",
            pieceImageReporter : "../resources/fartEmoji.png",
            pieceImageThug : "../resources/friesEmoji.png",
        };
    }
}