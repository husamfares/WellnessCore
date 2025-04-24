import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RecoveryService {
  baseUrl = 'http://localhost:5000/api/';

  constructor(private http: HttpClient) {}

  calculateRecovery(data: any): Observable<any> {
    return this.http.post<any>(this.baseUrl + 'recovery/calculate', data);
  }

  getRecoveryHistory(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + 'recovery/history');
  }
}

