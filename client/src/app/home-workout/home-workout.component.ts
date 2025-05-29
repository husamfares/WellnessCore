import { Component, HostListener, inject, OnInit } from '@angular/core';
import { UserService } from '../_services/user.service';
import { WorkoutService } from '../_services/workout.service';
import { BodyPartExercises } from '../_models/BodyPartExercises';
import { Exercise } from '../_models/Exercise';
import { CommonModule, NgFor, NgIf, TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-home-workout',
  imports: [NgIf , NgFor , TitleCasePipe,CommonModule],
  templateUrl: './home-workout.component.html',
  styleUrl: './home-workout.component.css'
})
export class HomeWorkoutComponent implements OnInit{

  userService = inject(UserService);
  workout = inject(WorkoutService);
  fitnessLevel : string ="";
  traineeGoal : string="";
  homeWorkout : BodyPartExercises[] = [];
  selectedExercises: Exercise[] = [];
  selectedExercise: Exercise | null = null;
  showBackToTop: boolean = false;
  selectedBodyPart: string = '';
  isLoading: boolean = false;
  instructionButton: boolean = false;

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
    this.isLoading = true;
    this.getFitnessCheckInfo();
    
}

getFitnessCheckInfo() {
    this.userService.getUserFitnessInfo().subscribe({
        next : data => {
            console.log("home page", data);
            this.fitnessLevel = data.fitnessLevel;
            this.traineeGoal = data.traineeGoal;

            //  Now that we have fitness info, fetch the workout
            this.getResultOfHomeWorkout();
        }, 
        error: (err) => {
            console.error('Error fetching fitness info:', err);    
        }  
    });
}


getResultOfHomeWorkout() {
  this.isLoading = true;
  this.workout.getHomeWorkout(this.fitnessLevel, this.traineeGoal).subscribe({
    next: listExercise => {
      console.log('Home workout loaded:', listExercise);
      this.homeWorkout = listExercise as BodyPartExercises[];
      this.isLoading = false;
    },
    error: (err) => {
      console.error('Error loading home workout:', err);
      this.isLoading = false;
    }
  });
}

instructionsOn()
{
  this.instructionButton = !this.instructionButton;
}

showHomeExercises(bodyPart: string) {
  this.selectedBodyPart = bodyPart;
  const part = this.homeWorkout.find(p => p.bodyPart.toLowerCase() === bodyPart.toLowerCase());
  this.selectedExercises = part?.exercises || [];

  // Scroll to exercise section
  setTimeout(() => {
    const exercisesSection = document.querySelector('.exercises-list');
    if (exercisesSection) {
      exercisesSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }, 100);
}

@HostListener('window:scroll')
checkScrollPosition() {
  this.showBackToTop = window.pageYOffset > 300;
}

scrollToTop() {
  window.scrollTo({ top: 0, behavior: 'smooth' });
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
