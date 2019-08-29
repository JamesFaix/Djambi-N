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
    faSearch
} from "@fortawesome/free-solid-svg-icons";

export default class Icons {
    public static readonly guest: IconDefinition = faIdBadge;

    public static readonly PlayerStatus = class {
        public static readonly Pending : IconDefinition = faSpinner;
        public static readonly Alive : IconDefinition = faHeart;
        public static readonly AcceptsDraw : IconDefinition = faHandshake;
        public static readonly Conceded : IconDefinition = faFlag;
        public static readonly Eliminated : IconDefinition = faSkull;
        public static readonly Victorious : IconDefinition = faTrophy;
    }

    public static readonly LobbyAction = class {
        public static readonly AddPlayer : IconDefinition = faPlus;
        public static readonly RemovePlayer : IconDefinition = faMinus;
    }

    public static readonly GameStatus = class {
        public static readonly Pending : IconDefinition = faSpinner;
        public static readonly InProgress : IconDefinition = faPlay;
        public static readonly Canceled : IconDefinition = faBan;
        public static readonly Over : IconDefinition = faAward;
    }

    public static readonly PlayerAction = class {
        public static readonly concede : IconDefinition = faFlag;
        public static readonly acceptDraw : IconDefinition = faHandshake;
        public static readonly revokeDraw : IconDefinition = faHandMiddleFinger;
        public static readonly endTurn : IconDefinition = faCheck;
        public static readonly resetTurn : IconDefinition = faRecycle;
    }

    public static readonly Page = class {
        public static readonly signup : IconDefinition = faUserPlus;
        public static readonly login : IconDefinition = faSignInAlt;
        public static readonly home : IconDefinition = faHome;
        public static readonly newGame : IconDefinition = faPlus;
        public static readonly lobby : IconDefinition = faDoorOpen;
        public static readonly play : IconDefinition = faPlay;
        public static readonly logout : IconDefinition = faSignOutAlt;
        public static readonly search : IconDefinition = faSearch;
        public static readonly diplomacy : IconDefinition = faHandshake;
    }
}