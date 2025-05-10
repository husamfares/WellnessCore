import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

export interface NutritionGuide {
  id: number;
  ageRangeStart: number;
  ageRangeEnd: number;
  gender: string;
  goal: string;
  calories: number;
  proteinGrams: number;
  carbsGrams: number;
  fatGrams: number;
  sunday: string;
  monday: string;
  tuesday: string;
  wednesday: string;
  thursday: string;
  friday: string;
  saturday: string;

  [key: string]: string | number;
}


@Injectable({
  providedIn: 'root'
})
export class NutritionService {
  private apiUrl = 'https://localhost:5001/api/NutritionGuides';

  constructor(private http: HttpClient) { }

  getNutritionGuide(username: string) {
    return this.http.post<NutritionGuide>(`${this.apiUrl}/get-user-guide`, { username });
  }
}

