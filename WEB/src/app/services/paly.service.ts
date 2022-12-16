import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { GameRequest } from '../modal/GameModal';

@Injectable( {
	providedIn: 'root'
} )
export class PalyService {
	public showSpinner: BehaviorSubject<boolean> = new BehaviorSubject(false);
	constructor (private http: HttpClient) { }

	requestNew(contestSelectedDoor:number, sessionId?: any): Observable<any> {
		this.showSpinner.next(true);
		return this.http.get(`${ environment.apiUrl }game/new?doorNumber=${ contestSelectedDoor }` + `&CurrentSessionID=${ sessionId }`)
		.pipe(
			tap(
			  (response) => this.showSpinner.next(false),
			  (error: any) => this.showSpinner.next(false)
			)
		)
	}

	getResult( request: GameRequest ): Observable<any> {
		return this.http.post( `${ environment.apiUrl }game/getResult`, request );
	}

	autoPlay(numOfRounds:number, isSwitch: boolean): Observable<any> {
		return this.http.get(`${ environment.apiUrl }game/autoPlay?numOfRounds=${ numOfRounds }` + `&isSwitch=${ isSwitch }`);
	}

	customPlay(numOfRounds:number, numOfSets: number): Observable<any> {
		return this.http.get(`${ environment.apiUrl }game/customPlay?numOfRounds=${ numOfRounds }` + `&numOfSets=${ numOfSets }`);
	}

}
