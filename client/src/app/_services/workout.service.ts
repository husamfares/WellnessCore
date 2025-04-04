import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WorkoutService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrL;
  
  getWorkouts(): Observable<any> {
    return this.http.get<any>(this.baseUrl);
  }
}
