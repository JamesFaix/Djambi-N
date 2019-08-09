import { createHashHistory } from 'history';

export const history = createHashHistory();

export function navigateTo(route : string) : void {
    history.push(route);
}