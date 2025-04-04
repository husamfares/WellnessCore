import { Component, inject } from '@angular/core';
import { WorkoutService } from '../_services/workout.service';

@Component({
  selector: 'app-workout-plans',
  imports: [],
  templateUrl: './workout-plans.component.html',
  styleUrl: './workout-plans.component.css'
})
export class WorkoutPlansComponent {
  private workoutService = inject(WorkoutService);


}
