export class GameRequest {
	ContestSelectedDoor!: number;
	HostOpenedDoor!: number;
	DoorWithCar!: number;
	SimulationType!: SimulationType;
	IsSwitched!: boolean;
}

export enum Result {
    Lost = 0,
    Won = 1
}

export enum ResultColor {
    Won = 'success',
    Lost = 'danger'
}

export enum SimulationType
{
   Single= 1,
   Auto = 2,
   Custom = 3
}


export class Response {
    RoundNumber!: number; 
    SimulationType!: SimulationType;
    DN_with_Car!: number;
}

export enum Switch
{
   Yes = 1,
   No = 0
}

