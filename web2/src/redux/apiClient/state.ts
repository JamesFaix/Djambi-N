export type ApiClientState = {
  pendingRequestIds: string[]
};

export const defaultApiClientState: ApiClientState = {
  pendingRequestIds: [],
};
