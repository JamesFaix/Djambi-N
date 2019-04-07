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
    private theme : Theme;

    constructor(){
        this.setTheme(ThemeFactory.voidTheme);
    }

    private setTheme(theme : Theme) : void {
        this.theme = theme;

        const docStyle = document.documentElement.style;

        docStyle.setProperty("--background-color", theme.pageStyle.backgroundColor);
        docStyle.setProperty("--text-color", theme.pageStyle.textColor);
        docStyle.setProperty("--border-color", theme.pageStyle.borderColor);
        docStyle.setProperty("--hint-text-color", theme.pageStyle.hintTextColor);

        docStyle.setProperty("--cell-color-even", theme.cellStyle.colorEven);
        docStyle.setProperty("--cell-color-odd", theme.cellStyle.colorOdd);
        docStyle.setProperty("--cell-text-color-even", theme.cellStyle.textColorEven);
        docStyle.setProperty("--cell-text-color-odd", theme.cellStyle.textColorOdd);
    }

    public getCellBaseColor(type : CellType) : string {
        switch(type) {
            case CellType.Center: return this.theme.cellStyle.colorCenter;
            case CellType.Even: return this.theme.cellStyle.colorEven;
            case CellType.Odd: return this.theme.cellStyle.colorOdd;
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
                    color: this.theme.cellHighlightStyle.selectedColor,
                    intensity: this.theme.cellHighlightStyle.selectedIntensity
                };

            case CellState.Selectable:
                return {
                    color: this.theme.cellHighlightStyle.selectionOptionColor,
                    intensity: this.theme.cellHighlightStyle.selectionOptionIntensity
                };

            default:
                throw "Invalid cell state.";
        }
    }

    public getCenterCellName() : string {
        return this.theme.centerCellName;
    }

    private getPieceImagePath(kind : PieceKind) : string {
        switch (kind) {
            case PieceKind.Assassin: return this.theme.pieces.imageAssassin;
            case PieceKind.Chief: return this.theme.pieces.imageChief;
            case PieceKind.Corpse: return this.theme.pieces.imageCorpse;
            case PieceKind.Diplomat: return this.theme.pieces.imageDiplomat;
            case PieceKind.Gravedigger: return this.theme.pieces.imageGravedigger;
            case PieceKind.Reporter: return this.theme.pieces.imageReporter;
            case PieceKind.Thug: return this.theme.pieces.imageThug;
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
            case PieceKind.Assassin: return this.theme.pieces.nameAssassin;
            case PieceKind.Chief: return this.theme.pieces.nameChief;
            case PieceKind.Corpse: return this.theme.pieces.nameCorpse;
            case PieceKind.Diplomat: return this.theme.pieces.nameDiplomat;
            case PieceKind.Gravedigger: return this.theme.pieces.nameGravedigger;
            case PieceKind.Reporter: return this.theme.pieces.nameReporter;
            case PieceKind.Thug: return this.theme.pieces.nameThug;
            default: throw "Invalid piece kind.";
        }
    }

    public getPlayerColor(colorId : number) : string {
        switch (colorId) {
            case 0: return this.theme.players.color0;
            case 1: return this.theme.players.color1;
            case 2: return this.theme.players.color2;
            case 3: return this.theme.players.color3;
            case 4: return this.theme.players.color4;
            case 5: return this.theme.players.color5;
            case 6: return this.theme.players.color6;
            case 7: return this.theme.players.color7;
            case null : return null; //Neutral
            default: throw "Invalid colorId. " + colorId;
        }
    }

    public getSelectionDescriptionTemplate(selection : Selection) : string {
        switch (selection.kind) {
            case SelectionKind.Drop:
                return this.theme.gameCopy.selectionDescriptionDrop;

            case SelectionKind.Move:
                return selection.pieceId === null
                    ? this.theme.gameCopy.selectionDescriptionMove
                    : this.theme.gameCopy.selectionDescriptionMoveAndTarget;

            case SelectionKind.Subject:
                return this.theme.gameCopy.selectionDescriptionSubject;

            case SelectionKind.Target:
                return this.theme.gameCopy.selectionDescriptionTarget;

            case SelectionKind.Vacate:
                return this.theme.gameCopy.selectionDescriptionVacate;

            default: throw "Invalid selection kind.";
        }
    }

    public getTurnPrompt(turn : Turn) : string {
        switch (turn.status) {
            case TurnStatus.AwaitingSelection:
                return this.getSelectionPrompt(turn.requiredSelectionKind);
            case TurnStatus.AwaitingCommit:
                return this.theme.gameCopy.turnPromptCommit;
            case TurnStatus.DeadEnd:
                return this.theme.gameCopy.turnPromptDeadEnd;
            default:
                throw "Invalid turn status.";
        }
    }

    private getSelectionPrompt(kind : SelectionKind) : string {
        switch (kind) {
            case SelectionKind.Drop: return this.theme.gameCopy.selectionPromptDrop;
            case SelectionKind.Move: return this.theme.gameCopy.selectionPromptMove;
            case SelectionKind.Subject: return this.theme.gameCopy.selectionPromptSubject;
            case SelectionKind.Target: return this.theme.gameCopy.selectionPromptTarget;
            case SelectionKind.Vacate: return this.theme.gameCopy.selectionPromptVacate;
            default: throw "Invalid selection kind.";
        }
    }

    public getEventMessageTemplate(event : Event) : string {
        switch (event.kind) {
            case EventKind.GameStarted:
                return this.theme.gameCopy.eventGameStarted;
            case EventKind.TurnCommitted:
                return this.theme.gameCopy.eventTurnCommitted;
            case EventKind.PlayerStatusChanged:
                return this.theme.gameCopy.eventPlayerStatusChanged;
            default:
                throw "Unsupported event kind.";
        }
    }

    public getEffectMessageTemplate(effect : Effect) : string {
        switch (effect.kind) {
            case EffectKind.GameStatusChanged:
                return this.getGameStatusChangedTemplate(effect);
            case EffectKind.NeutralPlayerAdded:
                return this.theme.gameCopy.effectNeutralPlayerAdded;
            case EffectKind.PieceAbandoned:
                return this.theme.gameCopy.effectPieceAbandoned;
            case EffectKind.PieceDropped:
                return this.theme.gameCopy.effectPieceDropped;
            case EffectKind.PieceEnlisted:
                return this.theme.gameCopy.effectPieceEnlisted;
            case EffectKind.PieceKilled:
                return this.theme.gameCopy.effectPieceKilled;
            case EffectKind.PieceMoved:
                return this.theme.gameCopy.effectPieceMoved;
            case EffectKind.PieceVacated:
                return this.theme.gameCopy.effectPieceVacated;
            case EffectKind.PlayerAdded:
                return this.theme.gameCopy.effectPlayerAdded;
            case EffectKind.PlayerOutOfMoves:
                return this.theme.gameCopy.effectPlayerOutOfMoves;
            case EffectKind.PlayerRemoved:
                return this.theme.gameCopy.effectPlayerRemoved;
            case EffectKind.PlayerStatusChanged:
                return this.getPlayerStatusChangedTemplate(effect);
            case EffectKind.TurnCycleAdvanced:
                return this.theme.gameCopy.effectTurnCycleAdvanced;
            case EffectKind.TurnCyclePlayerFellFromPower:
                return this.theme.gameCopy.effectTurnCyclePlayerFellFromPower;
            case EffectKind.TurnCyclePlayerRemoved:
                return this.theme.gameCopy.effectTurnCyclePlayerRemoved;
            case EffectKind.TurnCyclePlayerRoseToPower:
                return this.theme.gameCopy.effectTurnCyclePlayerRoseToPower;
            default:
                throw "Unsupported effect kind.";
        }
    }

    private getGameStatusChangedTemplate(effect : Effect) : string {
        const f = effect.value as GameStatusChangedEffect;

        switch (f.newValue) {
            case GameStatus.InProgress:
                return this.theme.gameCopy.effectGameStatusChangedInProgress;
            case GameStatus.Over:
                return this.theme.gameCopy.effectGameStatusChangedOver;
            default:
                throw "Unsupported game status.";
        }
    }

    private getPlayerStatusChangedTemplate(effect : Effect) : string {
        const f = effect.value as PlayerStatusChangedEffect;
        switch (f.newStatus) {
            case PlayerStatus.AcceptsDraw:
                return this.theme.gameCopy.effectPlayerStatusChangedAcceptsDraw;
            case PlayerStatus.Alive:
                return this.theme.gameCopy.effectPlayerStatusChangedAlive;
            case PlayerStatus.Conceded:
                return this.theme.gameCopy.effectPlayerStatusChangedConceded;
            case PlayerStatus.WillConcede:
                return this.theme.gameCopy.effectPlayerStatusChangedWillConcede;
            case PlayerStatus.Eliminated:
                return this.theme.gameCopy.effectPlayerStatusChangedEliminated;
            case PlayerStatus.Victorious:
                return this.theme.gameCopy.effectPlayerStatusChangedVictorious;
            default:
                throw "Unsupported player status.";
        }
    }
}