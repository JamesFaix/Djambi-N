import { UserDto } from '../../api-client';

export type SessionState = {
  user: UserDto | null
};

export const defaultSessionState = {
  user: null,
};
