import {
    IconDefinition,
    faIdBadge,
    faSpinner,
    faHeart,
    faHandshake,
    faFlag,
    faSkull,
    faTrophy,
    faUserPlus,
    faSignInAlt,
    faHome,
    faPlus,
    faDoorOpen,
    faSignOutAlt,
    faMinus,
    faHandMiddleFinger,
    faRecycle,
    faCheck,
    faPlay,
    faAward,
    faBan,
    faSearch,
    faScroll,
    faClock,
    faEllipsisH,
    faExclamation,
    faCamera,
    faSave,
    faTrashAlt,
    faCog
} from "@fortawesome/free-solid-svg-icons";
import { PlayerStatus, GameStatus } from "../api/model";

export interface IconInfo {
    icon : IconDefinition,
    title : string
}

export class Icons {
    private static readonly playerStatusIcons = new Map<PlayerStatus, IconInfo>([
        [PlayerStatus.AcceptsDraw, {
            icon: faHandshake,
            title: "Accepts draw"
        }],
        [PlayerStatus.Alive, {
            icon: faHeart,
            title: "Alive"
        }],
        [PlayerStatus.Conceded, {
            icon: faFlag,
            title: "Conceded"
        }],
        [PlayerStatus.Eliminated, {
            icon: faSkull,
            title: "Eliminated"
        }],
        [PlayerStatus.Pending, {
            icon: faSpinner,
            title: "Pending"
        }],
        [PlayerStatus.Victorious, {
            icon: faTrophy,
            title: "Victorious"
        }],
        [PlayerStatus.WillConcede, {
            icon: faFlag,
            title: "Will concede"
        }]
    ]);

    private static readonly gameStatusIcons = new Map<GameStatus, IconInfo>([
        [GameStatus.Canceled, {
            icon: faBan,
            title: "Canceled"
        }],
        [GameStatus.InProgress, {
            icon: faPlay,
            title: "In progress"
        }],
        [GameStatus.Over, {
            icon: faAward,
            title: "Over"
        }],
        [GameStatus.Pending, {
            icon: faSpinner,
            title: "Pending"
        }]
    ]);

    public static playerStatus(status : PlayerStatus) {
        return this.playerStatusIcons.get(status);
    }

    public static gameStatus(status : GameStatus) {
        return this.gameStatusIcons.get(status);
    }

    public static readonly Pages = class {
        public static readonly login : IconInfo = { icon: faSignInAlt, title: "Log in" };
        public static readonly home : IconInfo = { icon: faHome, title: "Home" };
        public static readonly newGame : IconInfo = { icon: faPlus, title: "Create game" };
        public static readonly lobby : IconInfo = { icon: faDoorOpen, title: "Lobby" };
        public static readonly play : IconInfo = { icon: faPlay, title: "Play" };
        public static readonly diplomacy : IconInfo = { icon: faHandshake, title: "Diplomacy" };
        public static readonly signup : IconInfo = { icon: faUserPlus, title: "Sign up" };
        public static readonly snapshots : IconInfo = { icon: faCamera, title: "Snapshots" };
        public static readonly settings : IconInfo = { icon: faCog, title: "Settings" };
        public static readonly gameOver : IconInfo = { icon: faAward, title: "Game results" };
    }

    public static readonly UserActions = class {
        public static readonly login : IconInfo = { icon: faSignInAlt, title: "Log in" };
        public static readonly signup: IconInfo = { icon: faUserPlus, title: "Create account" };
        public static readonly logout : IconInfo = { icon: faSignOutAlt, title: "Log out" };
        public static readonly search : IconInfo = { icon: faSearch, title: "Search" };
        public static readonly createGame : IconInfo = { icon: faPlus, title: "Create game" };
        public static readonly startGame : IconInfo = { icon: faPlay, title: "Start game" };
        public static readonly playerJoin : IconInfo = { icon: faPlus, title: "Join" };
        public static readonly playerAddGuest : IconInfo = { icon: faPlus, title: "Add guest" };
        public static readonly playerQuit : IconInfo = { icon: faMinus, title: "Quit" };
        public static readonly playerRemove : IconInfo = { icon: faMinus, title: "Remove" };
        public static readonly loadGame : IconInfo = { icon: faDoorOpen, title: "Load game" };
    }

    public static readonly PlayerActions = class {
        public static readonly concede : IconInfo = { icon: faFlag, title: "Concede" };
        public static readonly acceptDraw : IconInfo = { icon: faHandshake, title: "Accept draw" };
        public static readonly revokeDraw : IconInfo = { icon: faHandMiddleFinger, title: "Revoke draw" };
        public static readonly endTurn : IconInfo = { icon: faCheck, title: "Finish turn" };
        public static readonly resetTurn : IconInfo = { icon: faRecycle, title: "Reset turn" };
    }

    public static readonly PlayerNotes = class {
        public static readonly guest: IconDefinition = faIdBadge;
    }

    public static readonly Timeline = class {
        public static readonly history : IconInfo = { icon: faScroll, title: "History" };
        public static readonly turnCycle : IconInfo = { icon: faClock, title: "Turn cycle" };
        public static currentTurn(playerName : string, isCurrentUser : boolean) : IconInfo {
            return {
                icon: isCurrentUser ? faExclamation: faEllipsisH,
                title: `${playerName}'s turn`
            };
        }
    }

    public static readonly Snapshots = class {
        public static readonly save : IconInfo = { icon: faSave, title: "Save" };
        public static readonly load : IconInfo = { icon: faDoorOpen, title: "Load" };
        public static readonly delete : IconInfo = { icon: faTrashAlt, title: "Delete" };
    }
}