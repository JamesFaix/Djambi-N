import { UserDto } from '../../api-client';

export type SessionState = {
  user: UserDto | null,
  isRestorePending: boolean,
};

export const defaultSessionState: SessionState = {
  user: null,
  isRestorePending: true,
};
