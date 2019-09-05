import * as StoreActiveGame from '../store/activeGame';
import { Dispatch } from 'redux';
import { CreateSnapshotRequest } from '../api/model';
import * as Api from "../api/client";
import GameStoreFlows from './game';

export default class SnapshotStoreFlows {
    public static getSnapshots(gameId: number) {
        return async function(dispatch: Dispatch) : Promise<void> {
            const snapshots = await Api.getSnapshotsForGame(gameId);
            dispatch(StoreActiveGame.Actions.loadSnapshots(snapshots))
        }
    }

    public static saveSnapshot(gameId : number, request : CreateSnapshotRequest) {
        return async function(dispatch: Dispatch) : Promise<void> {
            const snapshot = await Api.createSnapshot(gameId, request);
            dispatch(StoreActiveGame.Actions.snapshotSaved(snapshot));
        }
    }

    public static loadSnapshot(gameId : number, snapshotId : number) {
        return async function(dispatch: Dispatch) : Promise<void> {
            await Api.loadSnapshot(gameId, snapshotId);
            return GameStoreFlows.loadGameFull(gameId)(dispatch);
        }
    }

    public static deleteSnapshot(gameId : number, snapshotId : number) {
        return async function(dispatch: Dispatch) : Promise<void> {
            await Api.loadSnapshot(gameId, snapshotId);
            dispatch(StoreActiveGame.Actions.snapshotDeleted(snapshotId));
        }
    }
}