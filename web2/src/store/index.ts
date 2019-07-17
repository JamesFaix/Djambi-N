import { combineReducers, createStore, applyMiddleware } from "redux";
import { sessionReducer } from "./sessionReducer";
import { gameReducer } from "./gameReducer";

const rootReducer = combineReducers({
    session: sessionReducer,
    game: gameReducer
});

export type AppState = ReturnType<typeof rootReducer>;

export default function configureStore() {

    return createStore(
        rootReducer,
    );
}