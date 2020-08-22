import {
  CreateUserRequestDto, ApiUsersPostRequest, ApiSessionsPostRequest, LoginRequestDto,
} from '../api-client';
import * as Api from '../utilities/api';
import {
  loggedIn, restoreSucceeded, restoreFailed,
} from '../redux/session/actionFactory';
import { store } from '../redux';

export async function signIn(request: LoginRequestDto): Promise<void> {
  const loginParams: ApiSessionsPostRequest = {
    loginRequestDto: {
      username: request.username,
      password: request.password,
    },
  };
  const session = await Api.sessions().apiSessionsPost(loginParams);

  const action = loggedIn(session.user);
  store.dispatch(action);
}

export async function createAccount(request: CreateUserRequestDto): Promise<void> {
  const createUserparams: ApiUsersPostRequest = {
    createUserRequestDto: {
      name: request.name,
      password: request.password,
    },
  };
  await Api.users().apiUsersPost(createUserparams);

  await signIn({
    username: request.name,
    password: request.password,
  });
}

export async function restoreSession(): Promise<void> {
  try {
    const user = await Api.users().apiUsersCurrentGet();
    const action = restoreSucceeded(user);
    store.dispatch(action);
  } catch {
    const action = restoreFailed();
    store.dispatch(action);
  }
}
