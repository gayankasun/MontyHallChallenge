export class Game {
	RoundNumber!: number;
	DN_with_Car!: number;
	DN_of_Player_Choice!: number;
	DN_of_Host_shows!: number;
	IsSwitch!: boolean;
    Result! : Result
}

export enum Result {
    Lost = 0,
    Won = 1
}

export enum SimulationType
{
   single= 1,
   Auto = 2,
   Custom = 3
}


export class Response {
    RoundNumber!: number; 
    SimulationType!: SimulationType;
    DN_with_Car!: number;
}

