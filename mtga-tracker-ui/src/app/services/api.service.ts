import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserInfo {
    mtgaInternalUserId: string;
    userNameWithCode: string;
    userName: string;
    userCode: string;
}

export interface Match {
    matchId: string;
    requestId: number;
    transactionId: string;
    timeStamp: string;
    matchCompletedReason: string;
    isDraw: boolean;
    winningTeamId: number;
    playerOneName: string;
    playerTwoName: string;
    playerOneMtgaId: string;
    playerTwoMtgaId: string;
    homeUser: string;
    winnerMtgArenaId: string;
    winnerName: string;
    user: UserInfo;
  }

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7145';

  getUsers(): Observable<UserInfo[]> {
    return this.http.get<UserInfo[]>(`${this.baseUrl}/api/UserInfo`);
  }

  getMatches(mtgArenaId: string): Observable<Match[]> {
    return this.http.get<Match[]>(`${this.baseUrl}/api/Match/${mtgArenaId}`);
  }
}