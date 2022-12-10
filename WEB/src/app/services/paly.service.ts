// import { QueryResultsModel } from './../../../models/request/query-results-model';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap, concatMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Game } from '../modal/GameModal';

@Injectable( {
	providedIn: 'root'
} )
export class PalyService {
  
	constructor (private httpService: HttpClient) { }

	forceLogOffByUserId( userId: number ): Observable<any> {
		return this.httpService.get( `${ environment.apiUrl }authentication/forceLogOffByUserId/?idValue=${ userId }` );
	}

	addUser( game: Game ): Observable<any> {
		return this.httpService.post( `${ environment.apiUrl }user/add`, Game );
	}

}
