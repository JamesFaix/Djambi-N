import {
  UserDto, GameDto, PlayerKind, PlayerDto,
} from '../../../api-client';

export enum PendingGamePlayerActionType {
  None,
  SelfJoin,
  AddGuest,
  Remove,
  SelfQuit
}

export type PendingGamePlayerSlotViewModel = {
  id: number | null,
  name: string,
  kind: PlayerKind | null,
  userId: number | null,
  note: string,
  actionType: PendingGamePlayerActionType
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
    const viewModel: PendingGamePlayerSlotViewModel = {
      id: p.id,
      name: p.name,
      kind: p.kind,
      userId: p.userId || null,
      note: getPlayerNote(p, game),
      actionType: PendingGamePlayerActionType.None,
    };

    if (p.userId === currentUser.id) {
      if (p.kind === PlayerKind.Guest) {
        // User can remove their own guests
        viewModel.actionType = PendingGamePlayerActionType.Remove;
      } else if (p.kind === PlayerKind.User) {
        // User can quit
        viewModel.actionType = PendingGamePlayerActionType.SelfQuit;
      }
    } else if (game.createdBy.userId === currentUser.id) {
      // Game creator can remove any player
      viewModel.actionType = PendingGamePlayerActionType.Remove;
    }

    return viewModel;
  });
}

export function getViewModels(currentUser: UserDto, game: GameDto)
  : PendingGamePlayerSlotViewModel[] {
  const viewModels = getViewModelsForCurrentPlayers(currentUser, game);
  const emptySlotCount = game.parameters.regionCount - viewModels.length;

  if (emptySlotCount > 0) {
    viewModels.push({
      id: null,
      name: '',
      kind: null,
      userId: null,
      note: '',
      actionType: PendingGamePlayerActionType.SelfJoin,
    });
  }

  const emptySlots: PendingGamePlayerSlotViewModel[] = [];

  for (let i = 0; i < emptySlotCount; i += 1) {
    emptySlots.push({
      id: null,
      name: '',
      kind: null,
      userId: null,
      note: '',
      actionType: PendingGamePlayerActionType.Add,
    });
  }

  return players.concat(emptySlots);
}
