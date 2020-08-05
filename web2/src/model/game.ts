import { PlayerKind, CreationSourceDto } from "../api-client"

export type PlayerInfo = {
  id: number;
  name: string;
  kind: PlayerKind;
  userId: number | null;
}

export type GameInfo = {
  id: number;
  description: string;
  players: PlayerInfo[];
  createdBy: CreationSourceDto;
}
