import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Game } from '../modal/GameModal';

@Injectable( {
	providedIn: 'root'
} )
export class PalyService {

	constructor (private http: HttpClient) { }

	requestNew(): Observable<any> {
		return this.http.get(`${ environment.apiUrl }game/new`);
	}

	addUser( game: Game ): Observable<any> {
		return this.http.post( `${ environment.apiUrl }user/add`, Game );
	}

}
