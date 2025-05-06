import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common'; 

interface NutritionGuide {
  id: number;
  ageRangeStart: number;
  ageRangeEnd: number;
  gender: string;
  goal: string;
  calories: number;
  proteinGrams: number;
  carbsGrams: number;
  fatGrams: number;
  mealPlan: string;
}

@Component({
  selector: 'app-nutrition-guide',
  templateUrl: './nutrition-guide.component.html',
  styleUrls: ['./nutrition-guide.component.css'],
  imports : [CommonModule]
})
export class NutritionGuideComponent implements OnInit {
  nutritionGuide: NutritionGuide | null = null;
  errorMessage: string = '';

  constructor(private http: HttpClient, private route: ActivatedRoute) {}

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('userId');
    if (userId) {
      this.http.get<NutritionGuide>(`https://localhost:5001/api/NutritionGuide/${userId}`)
        .subscribe({
          next: (data) => this.nutritionGuide = data,
          error: (err) => {
            this.errorMessage = 'No suitable nutrition plan found.';
            console.error(err);
          }
        });
    } else {
      this.errorMessage = 'User ID is not available.';
    }
  }
}
