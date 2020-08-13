import {
  PlayerKind,
  CreationSourceDto,
  GameStatus,
  Privilege,
} from '../api-client';

export type PlayerInfo = {
  id: number;
  name: string;
  kind: PlayerKind;
  userId: number | null;
};

export type GameInfo = {
  id: number;
  status: GameStatus;
  description: string;
  players: PlayerInfo[];
  createdBy: CreationSourceDto;
};

export type UserInfo = {
  id: number;
  name: string;
  privileges: Privilege[];
};
