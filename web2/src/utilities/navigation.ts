import * as Log from './log';

// eslint-disable-next-line import/prefer-default-export
export function navigateTo<T>(this: T, url: string): void {
  Log.info(`navigating to ${url}`);
}
