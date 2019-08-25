import { IconDefinition, faIdBadge, faSpinner, faHeart, faHandshake, faFlag, faSkull, faTrophy, faUserPlus, faSignInAlt, faHome, faPlus, faDoorOpen, faChessBoard, faSignOutAlt, faMinus, faHandMiddleFinger } from "@fortawesome/free-solid-svg-icons";

export default class Icons {
    public static readonly guest: IconDefinition = faIdBadge;

    public static readonly playerStatusPending : IconDefinition = faSpinner;
    public static readonly playerStatusAlive : IconDefinition = faHeart;
    public static readonly playerStatusAcceptsDraw : IconDefinition = faHandshake;
    public static readonly playerStatusConceded : IconDefinition = faFlag;
    public static readonly playerStatusEliminated : IconDefinition = faSkull;
    public static readonly playerStatusVictorious : IconDefinition = faTrophy;

    public static readonly signup : IconDefinition = faUserPlus;
    public static readonly login : IconDefinition = faSignInAlt;
    public static readonly home : IconDefinition = faHome;
    public static readonly newGame : IconDefinition = faPlus;
    public static readonly lobby : IconDefinition = faDoorOpen;
    public static readonly play : IconDefinition = faChessBoard;
    public static readonly logout : IconDefinition = faSignOutAlt;

    public static readonly join : IconDefinition = faPlus;
    public static readonly addGuest : IconDefinition = faPlus;
    public static readonly quit : IconDefinition = faMinus;
    public static readonly revokeDraw : IconDefinition = faHandMiddleFinger;
}