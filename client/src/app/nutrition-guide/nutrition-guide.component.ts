import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NutritionService, NutritionGuide } from '../_services/nutrition.service'; 
import { CommonModule, NgFor, NgIf, TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-nutrition-guide',
  standalone: true,
  templateUrl: './nutrition-guide.component.html',
  styleUrls: ['./nutrition-guide.component.css'],
  imports: [NgIf , TitleCasePipe , NgFor]
})
export class NutritionGuideComponent implements OnInit {
  nutritionGuide: NutritionGuide | null = null;
  errorMessage: string = '';

  dayKeys: string[] = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'];
mealsData: { [day: string]: { [meal: string]: string } } = {};

  constructor(
    private nutritionService: NutritionService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const username = this.route.snapshot.paramMap.get('username');

    if (username) {
      this.nutritionService.getNutritionGuide(username).subscribe({
        next: data => {
          this.nutritionGuide = data;

          this.dayKeys.forEach(day => {
  this.mealsData[day] = this.getMeals(data[day as keyof NutritionGuide] as string);
});

        
        },
        error: err => {
          this.errorMessage = 'No suitable nutrition plan found.';
          console.error(err);
        }
      });
    } else {
      this.errorMessage = 'Username is not available.';
    }
  }

  getMeals(dayText: string): { [meal: string]: string } {
  const meals = ["Breakfast", "Snack", "Lunch", "Snack2", "Dinner"];
  const results: { [meal: string]: string } = {
    Breakfast: '',
    Snack: '',
    Snack2: '',
    Lunch: '',
    Dinner: ''
  };

  if (!dayText) return results;

  const parts = dayText.split(';');

  parts.forEach(part => {
    meals.forEach(meal => {
      const label = meal.replace('2', ''); // snack2 => snack
      const regex = new RegExp(`^\\s*${label}:\\s*(.*)$`, 'i');
      const match = part.match(regex);
      if (match) {
        if (meal === 'Snack2') {
          results['Snack2'] = match[1].trim();
        } else {
          results[meal] = match[1].trim();
        }
      }
    });
  });

  return results;
}

}