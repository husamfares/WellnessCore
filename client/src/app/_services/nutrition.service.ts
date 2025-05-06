import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NutritionService {
  private apiUrl = 'https://localhost:5001/api/NutritionGuides';

  constructor(private http: HttpClient) { }

  getNutritionGuide(userId: number) {
    return this.http.post(`${this.apiUrl}/get-user-guide`, { userId });
  }
}
