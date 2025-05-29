import { Component, inject, OnInit } from '@angular/core';
import { WorkoutService } from '../_services/workout.service';
import { TherapistExercises } from '../_models/TherapistExercises';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-therapist-exercises',
  imports: [NgFor, NgIf],
  templateUrl: './therapist-exercises.component.html',
  styleUrl: './therapist-exercises.component.css'
})
export class TherapistExercisesComponent implements OnInit {
  
  workoutService = inject(WorkoutService);
  therapistExercisesList: TherapistExercises[] = [];
  YoutubeUrl: string = "";
  isLoading: boolean = true;

  // Array of exercise icons/emojis for visual appeal
  private exerciseIcons: string[] = [
    '💪', '🏃', '🧘', '🤸', '🏋️', '🚴', '🏊', '⚡', 
    '🎯', '🔥', '💯', '✨', '🌟', '💫', '🎪', '🎨'
  ];

  ngOnInit(): void {
    this.loadTherapistExercises();
  }

  private loadTherapistExercises(): void {
    this.isLoading = true;
    
    this.workoutService.getTherapyExercises().subscribe({
      next: (exercises) => {
        console.log("the exercises:", exercises);
        this.therapistExercisesList = exercises;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load therapist exercises:', error);
        this.isLoading = false;
      }
    });
  }

  /**
   * Get a consistent icon for each exercise based on its index
   * @param index - The index of the exercise in the array
   * @returns A string emoji icon
   */
  getExerciseIcon(index: number): string {
    return this.exerciseIcons[index % this.exerciseIcons.length];
  }

  /**
   * Optional: Method to open YouTube video in a modal or embedded player
   * You can extend this functionality as needed
   */
  openVideoPlayer(youtubeUrl: string): void {
    // This could be extended to open a modal with embedded video
    // For now, it just opens in a new tab (handled by the template)
    console.log('Opening video:', youtubeUrl);
  }
}