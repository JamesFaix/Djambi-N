export enum NotificationLevel {
  Info = 'Info',
  Warning = 'Warning',
  Error = 'Error'
}

export type Notification = {
  id: string,
  level: NotificationLevel,
  message: string,
  time: Date
};
