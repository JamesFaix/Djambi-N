import { ApiClientActionTypes } from './actionTypes';

export type RequestSentAction = {
  type: typeof ApiClientActionTypes.RequestSent,
  requestId: string
};

export type ResponseReceivedAction = {
  type: typeof ApiClientActionTypes.ResponseReceived,
  requestId: string
};

export type ApiClientAction =
  RequestSentAction |
  ResponseReceivedAction;
