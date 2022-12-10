import { Component } from '@angular/core';
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


  ngOnInit(): void {}

  clickOnDoor(doorNumber: number): void {
    switch (doorNumber) {
      case 1: {
        this.d1ImageSrc = this.d1ImageForSet;
        break;
      }
      case 2: {
        this.d2ImageSrc = this.d2ImageForSet;
        break;
      }
      case 3: {
        this.d3ImageSrc = this.d3ImageForSet;
        break;
      }
    }

  }

  reset(): void {
    this.d1ImageSrc = '../../../../assets/images/Close.png';
    this.d2ImageSrc = '../../../../assets/images/Close.png';
    this.d3ImageSrc = '../../../../assets/images/Close.png';

    this.playService.requestNew().subscribe((res: any) => {
      console.log(res);
      this.simulateDoors(res.dN_with_Car);
    });
  }

  simulateDoors(doorNumWithCar: number): void {
    switch (doorNumWithCar) {
      case 1: {
        this.d1ImageForSet = '../../../../assets/images/Open-Car.png';
        this.d2ImageForSet = '../../../../assets/images/Open-Goat.png';
        this.d3ImageForSet = '../../../../assets/images/Open-Goat.png';
        break;
      }
      case 2: {
        this.d2ImageForSet = '../../../../assets/images/Open-Car.png';
        this.d1ImageForSet = '../../../../assets/images/Open-Goat.png';
        this.d3ImageForSet = '../../../../assets/images/Open-Goat.png';
        break;
      }
      case 3: {
        this.d3ImageForSet = '../../../../assets/images/Open-Car.png';
        this.d2ImageForSet = '../../../../assets/images/Open-Goat.png';
        this.d1ImageForSet = '../../../../assets/images/Open-Goat.png';
        break;
      }
    }
  }
}
