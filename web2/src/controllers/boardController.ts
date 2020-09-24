import { store } from '../redux';
import { boardLoadedAction } from '../redux/boards/actionFactory';
import * as Api from '../utilities/api';

export async function loadBoard(regionCount: number) : Promise<void> {
  const request = { regionCount };
  const response = await Api.boards().apiBoardsRegionCountGet(request);
  const action = boardLoadedAction(response);
  store.dispatch(action);
}

export async function preloadAllBoards() : Promise<void> {
  const minRegionCount = 3;
  const maxRegionCount = 8;
  for (let i = minRegionCount; i <= maxRegionCount; i += 1) {
    // Fire and forget for concurrency
    loadBoard(i);
  }
}
