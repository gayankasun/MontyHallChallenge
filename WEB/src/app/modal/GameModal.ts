export class Game {
	RoundNumber: number;
	DN_with_Car: number;
	DN_of_Player_Choice: number;
	DN_of_Host_shows: number;
	IsSwitch: boolean;
    Result : Result
}

export enum Result {
    Lost = 0,
    Won = 1
}