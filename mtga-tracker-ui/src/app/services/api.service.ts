import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserInfo {
  userId: number;
  userNameWithCode: string;
  userName: string;
  userCode: string;
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
}