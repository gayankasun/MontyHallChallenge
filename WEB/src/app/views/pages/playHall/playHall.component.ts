import { Component  } from '@angular/core';
import { ToasterComponent, ToasterPlacement } from '@coreui/angular';
import { GameLog, GameRequest, GameSummary, Result, SimulationType } from 'src/app/modal/GameModal';
import { PalyService } from '../../../services/paly.service';
import { Guid } from "guid-typescript";
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';

@Component({
  selector: 'play-Hall',
  templateUrl: './playHall.component.html',
  styleUrls: ['./playHall.component.scss'],
})
export class playHallComponent {
  constructor(private playService: PalyService) {}
  loadingSub: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  d1ImageSrc: string = '../../../../assets/images/Close.png';
  d2ImageSrc: string = '../../../../assets/images/Close.png';
  d3ImageSrc: string = '../../../../assets/images/Close.png';

  d1ImageForSet: string = "";
  d2ImageForSet: string = "";
  d3ImageForSet: string = "";

  loading: boolean = false;
  visible = false;
  showToast: boolean = false;
  contestSelectedDoor: number = 0;
  hostOpenedDoorNumber : number = 0;
  doorNumberWithCar: number = 0;
  gameRequest: GameRequest;
  gameSummary: GameSummary;
  msgResult!: string;
  resultColor!: string;
  gameLogs : Array<GameLog> = [];
  sessionID: any;
  roundNumber: number = 0;
  isSwitched:boolean = false;
  numOfRoundsToRun: number = 0;
  numOfRoundsToCusRun: number = 0;
  numOfSetsToCusRun: number = 0;
  arrWinPercentageIfSwitch: Array<number> = [];
  arrWinPercentageIfKeep: Array<number> = [];
  barChartData : any;


  ngOnInit(): void {
    this.gameSummary = new GameSummary();
    this.gameSummary.Rounds = 0;
    this.gameSummary.WonCount = 0;
    this.gameSummary.LostCount = 0;
    this.gameSummary.WinningPercentage = 0;

    this.barChartData = {
      labels: [...this.arrWinPercentageIfSwitch].slice(0, this.numOfSetsToCusRun),
      datasets: [
        {
          label: 'N/A',
          backgroundColor: '#f87979',
          data: []
        },
        {
          label: 'N/A',
          backgroundColor: '#f87979',
          data: []
        }
      ]
    };
  }


  runGame(){ 
    this.loading = true;
    this.playService.autoPlay(this.numOfRoundsToRun,this.isSwitched).subscribe((res: any)=>{
      console.log(res.messageBody.content.gameSummary);
      this.setGameSummary(res.messageBody.content.gameSummary);
      let barChartData = res.messageBody.content.gameSummary;
      let numOfRounds = [barChartData.rounds]
      this.barChartData = {
        labels: [...numOfRounds].slice(0, 1),
        datasets: [
          {
            label: 'Won',
            backgroundColor: '#00C850',
            data: [barChartData.winningPercentage]
          },
          {
            label: 'Lost',
            backgroundColor: '#f87979',
            data: [(100 - barChartData.winningPercentage)]
          }
        ]
      };
      this.loading = false;
    })
  }

  customPlayMode(){
    this.loading = true;
    this.playService.customPlay(this.numOfRoundsToCusRun,this.numOfSetsToCusRun).subscribe((res: any)=>{
      console.log(res.messageBody.content);
      let dataArray = res.messageBody.content;
      this.loadChartData(dataArray.numOfRoundsList,dataArray.listWinPercentageIfSwitch ,
           dataArray.listWinPercentageIfKeep,dataArray.noOfSetsCount);
    this.loading = false;
      
    })
  }

  loadChartData(numberOfRoundList: any[], dataForSwitch: any[], dataForKeep: any[], numOfSets: number){
    this.barChartData = {
      labels: [...numberOfRoundList].slice(0, numOfSets),
      datasets: [
        {
          label: 'Switch Strategy',
          backgroundColor: '#00C850',
          data: dataForSwitch
        },
        {
          label: 'Keep Strategy',
          backgroundColor: '#f87979',
          data: dataForKeep
        }
      ]
    };
  }

  clickOnDoor(doorNumber: number): void {
    this.resetDoors();
    this.contestSelectedDoor = doorNumber;   
    this.showToast = false;
    let currentSessionId = Guid.createEmpty();

    if(this.sessionID) {currentSessionId = this.sessionID};
    this.loading = true;
    this.playService.requestNew(this.contestSelectedDoor, currentSessionId).subscribe((res: any) => {
      console.log(res.messageBody.content.game);
      this.hostOpenedDoorNumber = res.messageBody.content.game.dN_host_going_to_open;
      this.doorNumberWithCar = res.messageBody.content.game.dN_with_Car;
      this.sessionID = res.messageBody.content.game.sessionId;
      this.roundNumber = res.messageBody.content.game.roundNumber;
      this.arrangeDoors(this.doorNumberWithCar, this.hostOpenedDoorNumber);
      console.log(this.playService.showSpinner.value);
      this.loading = false
      this.visible = true;

    });

  }

  resetDoors(): void {
    this.d1ImageSrc = '../../../../assets/images/Close.png';
    this.d2ImageSrc = '../../../../assets/images/Close.png';
    this.d3ImageSrc = '../../../../assets/images/Close.png';
    this.showToast = false;
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
   this.gameRequest = new GameRequest();

    this.gameRequest.ContestSelectedDoor = this.contestSelectedDoor;
    this.gameRequest.HostOpenedDoor = this.hostOpenedDoorNumber;
    this.gameRequest.DoorWithCar = this.doorNumberWithCar; 
    this.gameRequest.IsSwitched = isSwitch;
    this.gameRequest.SimulationType = SimulationType.single;
    this.gameRequest.SessionId = this.sessionID;
    this.gameRequest.RoundNumber = this.roundNumber;

    this.loading = true;
    this.playService.getResult(this.gameRequest).subscribe((res: any) => {
      console.log(res.messageBody.content.gameResult);
      let gameLog =  new GameLog();
      gameLog.Round= res.messageBody.content.gameResult.roundNumber;
      gameLog.DoorWithCar= res.messageBody.content.gameResult.dN_with_Car;
      gameLog.ContestSelectedDoor= res.messageBody.content.gameResult.dN_Contest_Choice;
      gameLog.IsSwitched= res.messageBody.content.gameResult.isSwitch;
      gameLog.Result= res.messageBody.content.gameResult.result;

      this.gameLogs.push(gameLog);
      this.setGameSummary(res.messageBody.content.gameResult.gameSummary);
      
      this.visible = !this.visible;
      this.showResult(res.messageBody.content.gameResult.dN_with_Car);
      if(res.messageBody.content.gameResult.result == Result.Won){
        this.msgResult = 'You Won';
        this.resultColor = 'success';
      }else{
        this.msgResult = 'You Lost'
        this.resultColor = 'danger';
      } this.showToast = true;
      this.loading = false;
    });
  }

  showResult(doorNumWithCar: number): void {
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
        this.d1ImageForSet = '../../../../assets/images/Open-Goat.png';
        this.d2ImageForSet = '../../../../assets/images/Open-Goat.png';
        break;
      }      
    }

    this.d1ImageSrc = this.d1ImageForSet;
    this.d2ImageSrc = this.d2ImageForSet;
    this.d3ImageSrc = this.d3ImageForSet;

      setTimeout(() =>{
        this.resetDoors();
      },3000)
  }

  setGameSummary(gameSummary: any){
    this.gameSummary.Rounds = gameSummary.rounds;
    this.gameSummary.WonCount = gameSummary.wonCount;
    this.gameSummary.LostCount = gameSummary.lostCount;
    this.gameSummary.WinningPercentage = gameSummary.winningPercentage;
  }

}
