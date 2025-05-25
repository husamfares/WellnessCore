import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { MealAnalysis } from '../_models/MealAnalysis';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MealAnalyzerService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrL;
  
  analyzeImage(file: File): Observable<MealAnalysis> {
    const formData = new FormData();
    formData.append('image', file);
    return this.http.post<MealAnalysis>(this.baseUrl + 'mealAnalyzer/analyze-image', formData);
  }
  getHistory(): Observable<MealAnalysis[]> {
    return this.http.get<MealAnalysis[]>(this.baseUrl + 'mealAnalyzer/history');
  }
}
