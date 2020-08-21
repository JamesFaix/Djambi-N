import {
  UserDto, GameDto, PlayerKind, PlayerDto,
} from '../../../api-client';

export enum LobbyPlayerActionType {
  None,
  SelfJoin,
  AddGuest,
  Remove,
  SelfQuit
}

export type LobbyPlayerViewModel = {
  id: number | null,
  name: string,
  kind: PlayerKind | null,
  userId: number | null,
  note: string,
  actionType: LobbyPlayerActionType
};

function getPlayerNote(player: PlayerDto, game: GameDto): string {
  switch (player.kind) {
    case PlayerKind.Guest: {
      const host = game.players.find((p) => p.kind === PlayerKind.User
        && p.userId === player.userId);
      if (!host) { throw new Error('Host player for guest not found.'); }
      return `Guest of ${host.name}`;
    }
    case PlayerKind.Neutral:
      return 'Neutral';
    default:
      return '';
  }
}

function getViewModelsForCurrentPlayers(currentUser: UserDto, game: GameDto) {
  return game.players.map((p) => {
    const viewModel: LobbyPlayerViewModel = {
      id: p.id,
      name: p.name,
      kind: p.kind,
      userId: p.userId || null,
      note: getPlayerNote(p, game),
      actionType: LobbyPlayerActionType.None,
    };

    if (p.userId === currentUser.id) {
      if (p.kind === PlayerKind.Guest) {
        // User can remove their own guests
        viewModel.actionType = LobbyPlayerActionType.Remove;
      } else if (p.kind === PlayerKind.User) {
        // User can quit
        viewModel.actionType = LobbyPlayerActionType.SelfQuit;
      }
    } else if (game.createdBy.userId === currentUser.id) {
      // Game creator can remove any player
      viewModel.actionType = LobbyPlayerActionType.Remove;
    }

    return viewModel;
  });
}

export function getViewModels(currentUser: UserDto, game: GameDto)
  : LobbyPlayerViewModel[] {
  const viewModels = getViewModelsForCurrentPlayers(currentUser, game);
  const emptySlotCount = game.parameters.regionCount - viewModels.length;
  const userIsPlayer = game.players.map((p) => p.userId).includes(currentUser.id);

  if (emptySlotCount > 0) {
    if (!userIsPlayer) {
      viewModels.push({
        id: null,
        name: '',
        kind: null,
        userId: null,
        note: '',
        actionType: LobbyPlayerActionType.SelfJoin,
      });
    } else {
      viewModels.push({
        id: null,
        name: '',
        kind: null,
        userId: null,
        note: '',
        actionType: LobbyPlayerActionType.AddGuest,
      });
    }
  }

  for (let i = 1; i < emptySlotCount; i += 1) {
    viewModels.push({
      id: null,
      name: '',
      kind: null,
      userId: null,
      note: '',
      actionType: LobbyPlayerActionType.None,
    });
  }

  return viewModels;
}
