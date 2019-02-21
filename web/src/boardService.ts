import { Board } from "./api/model";
import ApiClient from "./api/client";

export default class BoardService {
    private readonly boardCache : any;

    constructor(
        private readonly api : ApiClient
    ) {
        this.boardCache = {};
    }

    public async getBoard (regionCount : number) : Promise<Board> {
        //The board returned from the API of each regionCount is always the same, so it can be cached.
        let b = this.boardCache[regionCount];
        if (!b) {
            b = await this.api.getBoard(regionCount);
            this.boardCache[regionCount] = b;
        }
        return b;
    }

    public getBoardIfCached(regionCount : number) : Board {
        return this.boardCache[regionCount];
    }
}