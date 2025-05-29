import { NgFor, NgIf, TitleCasePipe } from '@angular/common';
import { Component, inject, OnInit, HostListener } from '@angular/core';
import { WorkoutService } from '../_services/workout.service';
import { UserService } from '../_services/user.service';
import { BodyPartExercises } from '../_models/BodyPartExercises';
import { Exercise } from '../_models/Exercise';

@Component({
  selector: 'app-standard-workout-plan',
  imports: [NgIf, NgFor, TitleCasePipe],
  templateUrl: './standard-workout-plan.component.html',
  styleUrl: './standard-workout-plan.component.css'
})
export class StandardWorkoutPlanComponent implements OnInit {
  workoutServive = inject(WorkoutService);
  userService = inject(UserService);

  selectedExercises: Exercise[] = [];
  selectedBodyPart: string = "";
  selectedExercise: Exercise | null = null;
  traineeLevel: string = "";
  traineeGoal: string = "";
  isLoading: boolean = false;
  showBackToTop: boolean = false;

  workoutPlan: BodyPartExercises[] = [];

  muscleGroups = [
    { key: 'chest', name: 'Chest', image: 'https://res.cloudinary.com/djbpwawux/image/upload/v1746458982/494573625_1745712823491760_1025466217134637284_n_edof4i.jpg' },
    { key: 'back', name: 'Back', image: 'https://res.cloudinary.com/djbpwawux/image/upload/v1746460274/466783462_3053067041508077_3838981021328059774_n_xtu50m.jpg' },
    { key: 'legs', name: 'Legs', image: 'https://res.cloudinary.com/djbpwawux/image/upload/v1746458702/gabin-vallet-JVnLqWGWVzs-unsplash_morbia.jpg' },
    { key: 'arms', name: 'Arms', image: 'https://res.cloudinary.com/djbpwawux/image/upload/v1746460904/494573636_699991205736083_1883892696673616374_n_pl3w2c.jpg' },
    { key: 'shoulders', name: 'Shoulders', image: 'https://res.cloudinary.com/djbpwawux/image/upload/v1746458743/shan-a-rajpoot-QkK7NtsPrJM-unsplash_hmhizy.jpg' },
    { key: 'core', name: 'Core', image: 'https://res.cloudinary.com/djbpwawux/image/upload/v1746459494/494579076_933023678807345_2025579482182602516_n_sdqgqm.jpg' },
    { key: 'cardio', name: 'Cardio', image: 'https://res.cloudinary.com/djbpwawux/image/upload/v1746460278/494574592_702733182174902_5468408608772892143_n_syn40j.jpg' }
  ]

  ngOnInit(): void {
    this.loadUserWorkoutPlan();
    this.checkScrollPosition();
  }

  @HostListener('window:scroll')
  checkScrollPosition() {
    this.showBackToTop = window.pageYOffset > 300;
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  loadUserWorkoutPlan() {
    this.isLoading = true;
    this.userService.getUserFitnessInfo().subscribe({
      next: fitnessInfo => {
        this.traineeLevel = fitnessInfo.fitnessLevel;
        this.traineeGoal = fitnessInfo.traineeGoal;

        this.workoutServive.getUserWorkout(this.traineeLevel, this.traineeGoal).subscribe({
          next: (data) => {
            this.workoutPlan = data as BodyPartExercises[];
            console.log('Workout plan loaded:', this.workoutPlan);
            this.isLoading = false;
          },
          error: (err) => {
            console.error("Error loading workout plan", err);
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        console.error('Error fetching fitness info:', err);
        this.isLoading = false;
      }
    });
  }

  showExercises(bodyPart: string) {
    this.selectedBodyPart = bodyPart;
    const part = this.workoutPlan.find(p => p.bodyPart.toLowerCase() === bodyPart.toLowerCase());
    this.selectedExercises = part?.exercises || [];
    
    // Scroll to exercises section
    setTimeout(() => {
      const exercisesSection = document.querySelector('.exercises-list');
      if (exercisesSection) {
        exercisesSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }, 100);
  }

  handleImageError(event: any) {
    event.target.src = 'assets/images/exercise-placeholder.jpg';
  }
  
  openExerciseModal(exercise: Exercise) {
    this.selectedExercise = exercise;
    // Prevent body scrolling when modal is open
    document.body.style.overflow = 'hidden';
  }
  
  closeExerciseModal() {
    this.selectedExercise = null;
    // Re-enable body scrolling
    document.body.style.overflow = '';
  }
  
  @HostListener('document:keydown.escape')
  onEscapeKey() {
    if (this.selectedExercise) {
      this.closeExerciseModal();
    }
  }
}