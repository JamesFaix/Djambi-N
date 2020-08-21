import { GamesQueryDto, SearchGameDto, ApiSearchGamesPostRequest } from '../api-client';
import * as Api from '../utilities/api';

export async function searchGames(query: GamesQueryDto): Promise<SearchGameDto[]> {
  const request: ApiSearchGamesPostRequest = { gamesQueryDto: query };
  const games = await Api.search().apiSearchGamesPost(request);
  return games;
}
