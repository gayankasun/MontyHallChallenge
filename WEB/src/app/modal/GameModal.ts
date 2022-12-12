export class GameRequest {
  RoundNumber!: number;
  ContestSelectedDoor!: number;
  HostOpenedDoor!: number;
  DoorWithCar!: number;
  SimulationType!: SimulationType;
  IsSwitched!: boolean;
  SessionId: any;
}

export enum Result {
  Lost = 0,
  Won = 1,
}

export enum SimulationType {
  single = 1,
  Auto = 2,
  Custom = 3,
}

export class Response {
  RoundNumber!: number;
  SimulationType!: SimulationType;
  DN_with_Car!: number;
}

export enum Switch {
  Yes = 1,
  No = 0,
}

export class GameLog {
  ContestSelectedDoor!: number;
  DoorWithCar!: number;
  IsSwitched!: boolean;
  Round: number = 0;
  Result!: Result;
}

export class GameSummary {
    SessionId :any;
    Rounds! : number;
    WonCount! : number;
    LostCount! : number;
    WinningPercentage! : number;
}
