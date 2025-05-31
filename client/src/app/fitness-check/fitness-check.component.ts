import { NgIf } from '@angular/common';
import { Component, Inject, inject, OnInit } from '@angular/core';
import { FormsModule, NgModel } from '@angular/forms';
import { UserService } from '../_services/user.service';
import { Member } from '../_models/member';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
@Component({
  selector: 'app-fitness-check',
  imports: [NgIf , FormsModule , CommonModule ],
  templateUrl: './fitness-check.component.html',
  styleUrl: './fitness-check.component.css'
})
export class FitnessCheckComponent implements OnInit{
  step = 0;
  pushUpsResult: number = 0;
  squatsResult: number = 0;
  plankResult: number = 0;
  currentResult: number=0;

  // Timer logic
  timeLeft: number = 0;
  isTiming: boolean = false;
  showResults: boolean = false;
  totalScore: number = 0;
  fitnessLevel : string = "";
  num : number=10;
  interval: any; // Save the interval ID here
  hasTimerStarted: boolean = false; 
  exerciseCompleted: boolean = false; // New flag
  traineeGoal : string ="";
  goalConfirmed: boolean = false;
  showSubmitClick:  boolean = false;

  allDataMember: Member = {
    age: 25,
    id: 0,
    username: '',
    weight: 0,
    height: 0,
    gender: '',
    fitnessLevel: '',
    traineeGoal: '',

  };
progressPercentage: any;
isCountingUp: any;
elapsedTime: any;

constructor(
  private userService: UserService,
  private router: Router
) {}

  
  ngOnInit(): void {
    this.userService.getUserFitnessInfo().subscribe( data => 
      {
        this.allDataMember = data;
      });
    
  }

  confirmGoal()
  {
    if(this.traineeGoal)
    this.goalConfirmed = true;
    this,this.showSubmitClick = true;
    console.log("Goal confirmed:", this.traineeGoal);
  }

  toggleGoal(selectedGoal : string)
  {
    this.traineeGoal = this.traineeGoal === selectedGoal ? '' : selectedGoal;
  }


  startFitnessCheck() {
    this.step = 1;
  }

  startTimer() {
    this.hasTimerStarted = true;
    this.isTiming = true;
    this.exerciseCompleted = false;
    this.timeLeft = 30;
    this.interval = setInterval(() => {
      this.timeLeft--;
      if (this.timeLeft <= 0) {
        clearInterval(this.interval);
        this.isTiming = false;
        this.exerciseCompleted = true; 
      }
    }, 1000);
  }

  stopTimer() {
    this.isTiming = false;
    clearInterval(this.interval);
    this.exerciseCompleted = true;

    console.log('Timer stopped');
  }
  nextStep() {
    this.exerciseCompleted = false;
    this.hasTimerStarted = false;
    
    // Save the current result to the appropriate variable
    if (this.step === 1) {
      this.pushUpsResult = this.currentResult;
    } else if (this.step === 2) {
      this.squatsResult = this.currentResult;
    } else if (this.step === 3) {
      this.plankResult = this.currentResult;
    }
    
    this.timeLeft = 30;
    this.currentResult = 0;
  
    if (this.step < 3) {
      this.step++;  // Go to next step
    } else {
      this.step = 4;  // Go to summary
    }
  }

  calculateTotalScore() {
    this.totalScore = this.pushUpsResult + this.squatsResult + this.plankResult;
  }

  updateFitnessLevel(): void {
    if (this.allDataMember && typeof this.allDataMember.age === 'number') 
      {
      this.fitnessLevel = this.getFitnessLevel(this.allDataMember); // Set the fitness level
      }
      else
      {
        this.fitnessLevel = "Unknown"; // fallback
      }
  }

  getFitnessLevel(data: Member): string {
    const age = data.age;
    console.log('age' , age);
    const score = this.totalScore;
  
    if (age < 18) {
      if (score < 50) return "Beginner";
      else if (score < 100) return "Moderate";
      else return "Advanced";
    } 
    else if (age >= 18 && age <= 29) {
      if (score < 61) return "Beginner";
      else if (score < 121) return "Moderate";
      else return "Advanced";
    } 
    else if (age >= 30 && age <= 39) {
      if (score < 56) return "Beginner";
      else if (score < 111) return "Moderate";
      else return "Advanced";
    } 
    else if (age >= 40 && age <= 49) {
      if (score < 51) return "Beginner";
      else if (score < 101) return "Moderate";
      else return "Advanced";
    } 
    else if (age >= 50) {
      if (score < 46) return "Beginner";
      else if (score < 91) return "Moderate";
      else return "Advanced";
    }
  
    return "Unknown";
  }
  
  
  submitFitnessCheck() {
      // Calculate total score and update fitness level
      this.calculateTotalScore();
      this.updateFitnessLevel();
      this.showResults = true; // Show results
    
      // Only perform the PUT request to update the fitness level
      this.userService.setFitnessLevel(this.fitnessLevel , this.traineeGoal).subscribe({
        next: () => {
          console.log('Fitness level updated successfully');
        },
        error: (err) => {
          console.error('Error updating fitness level:', err);
        },
      });
    }

    goHome(){
      this.router.navigate(['/home']);
    }
  }    

