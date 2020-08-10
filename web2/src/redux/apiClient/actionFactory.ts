import { RequestSentAction, ResponseReceivedAction } from './actions';
import { ApiClientActionTypes } from './actionTypes';

export function requestSent(requestId: string): RequestSentAction {
  return {
    type: ApiClientActionTypes.RequestSent,
    requestId,
  };
}

export function responseReceived(requestId: string): ResponseReceivedAction {
  return {
    type: ApiClientActionTypes.ResponseReceived,
    requestId,
  };
}
