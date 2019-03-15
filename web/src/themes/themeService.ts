import Theme from './theme';
import ThemeFactory from './themeFactory';
import { CellHighlight, CellState, CellType } from '../boardRendering/model';
import {
    Effect,
    EffectKind,
    Event,
    EventKind,
    PieceKind,
    Selection,
    SelectionKind,
    PlayerStatusChangedEffect,
    PlayerStatus,
    GameStatusChangedEffect,
    GameStatus,
    Turn,
    TurnStatus
    } from '../api/model';

export default class ThemeService {
    theme : Theme;
    private readonly defaultTheme : Theme;

    constructor(){
        this.defaultTheme = ThemeFactory.getDefaultTheme();
    }

    //Get the custom theme value if it exists, otherwise default value
    private getValue<T>(getProperty : (t : Theme) => T) : T {
        if (this.theme){
            const value = getProperty(this.theme);
            if (value) {
                return value;
            }
        }

        return getProperty(this.defaultTheme);
    }

    public getCellBaseColor(type : CellType) : string {
        switch(type) {
            case CellType.Center: return this.getValue(t => t.cellColorCenter);
            case CellType.Even: return this.getValue(t => t.cellColorEven);
            case CellType.Odd: return this.getValue(t => t.cellColorOdd);
            default: throw "Invalid cell type.";
        }
    }

    public getCellHighlight(state : CellState) : CellHighlight {
        switch (state)
        {
            case CellState.Default:
                return null;

            case CellState.Selected:
                return {
                    color: this.getValue(t => t.cellHighlightSelectedColor),
                    intensity: this.getValue(t => t.cellHighlightSelectedIntensity)
                };

            case CellState.Selectable:
                return {
                    color: this.getValue(t => t.cellHighlightSelectionOptionColor),
                    intensity: this.getValue(t => t.cellHighlightSelectionOptionIntensity)
                };

            default:
                throw "Invalid cell state.";
        }
    }

    public getCenterCellName() : string {
        return this.getValue(t => t.centerCellName);
    }

    private getPieceImagePath(kind : PieceKind) : string {
        switch (kind) {
            case PieceKind.Assassin: return this.getValue(t => t.pieceImageAssassin);
            case PieceKind.Chief: return this.getValue(t => t.pieceImageChief);
            case PieceKind.Corpse: return this.getValue(t => t.pieceImageCorpse);
            case PieceKind.Diplomat: return this.getValue(t => t.pieceImageDiplomat);
            case PieceKind.Gravedigger: return this.getValue(t => t.pieceImageGravedigger);
            case PieceKind.Reporter: return this.getValue(t => t.pieceImageReporter);
            case PieceKind.Thug: return this.getValue(t => t.pieceImageThug);
            default: throw "Invalid piece kind.";
        }
    }

    public getPieceImage(kind : PieceKind) : any {
        const image = new (window as any).Image();
        image.src = this.getPieceImagePath(kind);
        return image;
    }

    public getPieceName(kind : PieceKind) : string {
        switch (kind) {
            case PieceKind.Assassin: return this.getValue(t => t.pieceNameAssassin);
            case PieceKind.Chief: return this.getValue(t => t.pieceNameChief);
            case PieceKind.Corpse: return this.getValue(t => t.pieceNameCorpse);
            case PieceKind.Diplomat: return this.getValue(t => t.pieceNameDiplomat);
            case PieceKind.Gravedigger: return this.getValue(t => t.pieceNameGravedigger);
            case PieceKind.Reporter: return this.getValue(t => t.pieceNameReporter);
            case PieceKind.Thug: return this.getValue(t => t.pieceNameThug);
            default: throw "Invalid piece kind.";
        }
    }

    public getPlayerColor(colorId : number) : string {
        switch (colorId) {
            case 0: return this.getValue(t => t.playerColor0);
            case 1: return this.getValue(t => t.playerColor1);
            case 2: return this.getValue(t => t.playerColor2);
            case 3: return this.getValue(t => t.playerColor3);
            case 4: return this.getValue(t => t.playerColor4);
            case 5: return this.getValue(t => t.playerColor5);
            case 6: return this.getValue(t => t.playerColor6);
            case 7: return this.getValue(t => t.playerColor7);
            case null : return null; //Neutral
            default: throw "Invalid colorId. " + colorId;
        }
    }

    public getSelectionDescriptionTemplate(selection : Selection) : string {
        switch (selection.kind) {
            case SelectionKind.Drop:
                return this.getValue(t => t.selectionDescriptionDrop);

            case SelectionKind.Move:
                if (selection.pieceId === null) {
                    return this.getValue(t => t.selectionDescriptionMove);
                } else {
                    return this.getValue(t => t.selectionDescriptionMoveAndTarget);
                }

            case SelectionKind.Subject:
                return this.getValue(t => t.selectionDescriptionSubject);

            case SelectionKind.Target:
                return this.getValue(t => t.selectionDescriptionTarget);

            case SelectionKind.Vacate:
                return this.getValue(t => t.selectionDescriptionVacate);

            default: throw "Invalid selection kind.";
        }
    }

    public getTurnPrompt(turn : Turn) : string {
        switch (turn.status) {
            case TurnStatus.AwaitingSelection:
                return this.getSelectionPrompt(turn.requiredSelectionKind);
            case TurnStatus.AwaitingCommit:
                return this.getValue(t => t.turnPromptCommit);
            case TurnStatus.DeadEnd:
                return this.getValue(t => t.turnPromptDeadEnd);
            default:
                throw "Invalid turn status.";
        }
    }

    private getSelectionPrompt(kind : SelectionKind) : string {
        switch (kind) {
            case SelectionKind.Drop: return this.getValue(t => t.selectionPromptDrop);
            case SelectionKind.Move: return this.getValue(t => t.selectionPromptMove);
            case SelectionKind.Subject: return this.getValue(t => t.selectionPromptSubject);
            case SelectionKind.Target: return this.getValue(t => t.selectionPromptTarget);
            case SelectionKind.Vacate: return this.getValue(t => t.selectionPromptVacate);
            case null: return this.getValue(t => t.selectionPromptNull);
            default: throw "Invalid selection kind.";
        }
    }

    public getEventMessageTemplate(event : Event) : string {
        switch (event.kind) {
            case EventKind.GameStarted:
                return this.getValue(t => t.eventMessageGameStarted);
            case EventKind.TurnCommitted:
                return this.getValue(t => t.eventMessageTurnCommitted);
            case EventKind.PlayerStatusChanged:
                return this.getValue(t => t.eventMessagePlayerStatusChanged);
            default:
                throw "Unsupported event kind.";
        }
    }

    public getEffectMessageTemplate(effect : Effect) : string {
        switch (effect.kind) {
            case EffectKind.GameStatusChanged:
                return this.getGameStatusChangedMessageTemplate(effect);
            case EffectKind.NeutralPlayerAdded:
                return this.getValue(t => t.effectMessageNeutralPlayerAdded);
            case EffectKind.PieceAbandoned:
                return this.getValue(t => t.effectMessagePieceAbandoned);
            case EffectKind.PieceDropped:
                return this.getValue(t => t.effectMessagePieceDropped);
            case EffectKind.PieceEnlisted:
                return this.getValue(t => t.effectMessagePieceEnlisted);
            case EffectKind.PieceKilled:
                return this.getValue(t => t.effectMessagePieceKilled);
            case EffectKind.PieceMoved:
                return this.getValue(t => t.effectMessagePieceMoved);
            case EffectKind.PieceVacated:
                return this.getValue(t => t.effectMessagePieceVacated);
            case EffectKind.PlayerAdded:
                return this.getValue(t => t.effectMessagePlayerAdded);
            case EffectKind.PlayerOutOfMoves:
                return this.getValue(t => t.effectMessagePlayerOutOfMoves);
            case EffectKind.PlayerRemoved:
                return this.getValue(t => t.effectMessagePlayerRemoved);
            case EffectKind.PlayerStatusChanged:
                return this.getPlayerStatusChangedMessageTemplate(effect);
            case EffectKind.TurnCycleAdvanced:
                return this.getValue(t => t.effectMessageTurnCycleAdvanced);
            case EffectKind.TurnCyclePlayerFellFromPower:
                return this.getValue(t => t.effectMessageTurnCyclePlayerFellFromPower);
            case EffectKind.TurnCyclePlayerRemoved:
                return this.getValue(t => t.effectMessageTurnCyclePlayerRemoved);
            case EffectKind.TurnCyclePlayerRoseToPower:
                return this.getValue(t => t.effectMessageTurnCyclePlayerRoseToPower);
            default:
                throw "Unsupported effect kind.";
        }
    }

    private getGameStatusChangedMessageTemplate(effect : Effect) : string {
        const f = effect.value as GameStatusChangedEffect;

        switch (f.newValue) {
            case GameStatus.InProgress:
                return this.getValue(t => t.effectMessageGameStatusChangedInProgress);
            case GameStatus.Over:
                return this.getValue(t => t.effectMessageGameStatusChangedOver);
            default:
                throw "Unsupported game status.";
        }
    }

    private getPlayerStatusChangedMessageTemplate(effect : Effect) : string {
        const f = effect.value as PlayerStatusChangedEffect;
        switch (f.newStatus) {
            case PlayerStatus.AcceptsDraw:
                return this.getValue(t => t.effectMessagePlayerStatusChangedAcceptsDraw);
            case PlayerStatus.Alive:
                return this.getValue(t => t.effectMessagePlayerStatusChangedAlive);
            case PlayerStatus.Conceded:
                return this.getValue(t => t.effectMessagePlayerStatusChangedConceded);
            case PlayerStatus.WillConcede:
                return this.getValue(t => t.effectMessagePlayerStatusChangedWillConcede);
            case PlayerStatus.Eliminated:
                return this.getValue(t => t.effectMessagePlayerStatusChangedEliminated);
            case PlayerStatus.Victorious:
                return this.getValue(t => t.effectMessagePlayerStatusChangedVictorious);
            default:
                throw "Unsupported player status.";
        }
    }
}