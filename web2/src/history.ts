import { createHashHistory } from 'history';

export const history = createHashHistory();

export function navigateTo(route : string) : void {
 //   console.log("navigate to " + route);
    history.push(route);
}