import * as Log from './log';

export function navigateTo<T>(this: T, url: string): void {
  Log.info('navigating to ' + url);

};
