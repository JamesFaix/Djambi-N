/* eslint-disable no-console */

export const info = (message: string, ...args: any[]): void => {
  console.log(message, args);
};

export const warn = (message: string, ...args: any[]): void => {
  console.warn(message, args);
};

export const error = (message: string, ...args: any[]): void => {
  console.error(message, args);
};
