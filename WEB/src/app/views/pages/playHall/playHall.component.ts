import { Component } from '@angular/core';
import { request } from 'http';
import { GameRequest, SimulationType, Switch } from 'src/app/modal/GameModal';
import { PalyService } from '../../../services/paly.service';

@Component({
  selector: 'play-Hall',
  templateUrl: './playHall.component.html',
  styleUrls: ['./playHall.component.scss'],
})
export class playHallComponent {
  constructor(private playService: PalyService) {}

  d1ImageSrc: string = '../../../../assets/images/Close.png';
  d2ImageSrc: string = '../../../../assets/images/Close.png';
  d3ImageSrc: string = '../../../../assets/images/Close.png';

  d1ImageForSet: string = "";
  d2ImageForSet: string = "";
  d3ImageForSet: string = "";

  visible = false;
  contestSelectedDoor: number = 0;
  hostOpenedDoorNumber : number = 0;
  doorNumberWithCar: number = 0;
  gameRequest: GameRequest;

  ngOnInit(): void {}

  clickOnDoor(doorNumber: number): void {
    switch (doorNumber) {
      case 1: {
        this.contestSelectedDoor = doorNumber;
        break;
      }
      case 2: {
        this.contestSelectedDoor = doorNumber;
        break;
      }
      case 3: {
        this.contestSelectedDoor = doorNumber;
        break;
      }
    }

    this.playService.requestNew().subscribe((res: any) => {
      console.log(res);
      this.hostOpenedDoorNumber = res.dN_host_will_open;
      this.doorNumberWithCar = res.dN_with_Car;
      this.arrangeDoors(res.dN_with_Car, res.dN_host_will_open);
    });

    this.visible = true;

  }

  resetDoors(): void {
    this.d1ImageSrc = '../../../../assets/images/Close.png';
    this.d2ImageSrc = '../../../../assets/images/Close.png';
    this.d3ImageSrc = '../../../../assets/images/Close.png';
  }

  arrangeDoors(doorNumWithCar: number, doorNumHostWillOpen: number): void {
    switch (doorNumWithCar) {
      case 1: {
        this.d1ImageForSet = '../../../../assets/images/Open-Car.png';
        break;
      }
      case 2: {
        this.d2ImageForSet = '../../../../assets/images/Open-Car.png';
        break;
      }
      case 3: {
        this.d3ImageForSet = '../../../../assets/images/Open-Car.png';
        break;
      }
    }

    switch (doorNumHostWillOpen) {
      case 1: {
        this.d1ImageForSet = '../../../../assets/images/Open-Goat.png';
        this.d1ImageSrc = this.d1ImageForSet;
        break;
      }
      case 2: {
        this.d2ImageForSet = '../../../../assets/images/Open-Goat.png';
        this.d2ImageSrc = this.d2ImageForSet;
        break;
      }
      case 3: {
        this.d3ImageForSet = '../../../../assets/images/Open-Goat.png';
        this.d3ImageSrc = this.d3ImageForSet;
        break;
      }
    }
  }

  handleSwitchModel(event: any) {
    this.visible = event;
  }

  switch(isSwitch: boolean){
    console.log(isSwitch);

   this.gameRequest = new GameRequest();

    this.gameRequest.ContestSelectedDoor = this.contestSelectedDoor;
    this.gameRequest.HostOpenedDoor = this.hostOpenedDoorNumber;
    this.gameRequest.DoorWithCar = this.doorNumberWithCar; 
    this.gameRequest.IsSwitched = isSwitch;
    this.gameRequest.SimulationType = SimulationType.Single

    this.playService.getResult(this.gameRequest).subscribe((res: any) => {
      console.log(res);

      this.visible = !this.visible;
    });
  }

}
