import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GameRequest } from '../modal/GameModal';

@Injectable( {
	providedIn: 'root'
} )
export class PalyService {

	constructor (private http: HttpClient) { }

	requestNew(contestSelectedDoor:number, sessionId?: any): Observable<any> {
		return this.http.get(`${ environment.apiUrl }game/new?doorNumber=${ contestSelectedDoor }` + `&CurrentSessionID=${ sessionId }`);
	}

	getResult( request: GameRequest ): Observable<any> {
		return this.http.post( `${ environment.apiUrl }game/getResult`, request );
	}

	autoPlay(numOfRounds:number, isSwitch: boolean): Observable<any> {
		return this.http.get(`${ environment.apiUrl }game/autoPlay?numOfRounds=${ numOfRounds }` + `&isSwitch=${ isSwitch }`);
	}

}
