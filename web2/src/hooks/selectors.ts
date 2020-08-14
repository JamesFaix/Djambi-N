/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { RootState } from '../redux/root';

export const selectActiveGame = (state: RootState) => state.activeGame;

export const selectApiClient = (state: RootState) => state.apiClient;

export const selectConfig = (state: RootState) => state.config;

export const selectNavigation = (state: RootState) => state.navigation;

export const selectSession = (state: RootState) => state.session;
