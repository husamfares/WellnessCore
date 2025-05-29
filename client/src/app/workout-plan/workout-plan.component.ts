import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterModule } from '@angular/router';

@Component({
  selector: 'app-workout-plan',
  imports: [RouterLink, RouterModule , CommonModule],
  templateUrl: './workout-plan.component.html',
  styleUrl: './workout-plan.component.css'
})
export class WorkoutPlanComponent {

}
