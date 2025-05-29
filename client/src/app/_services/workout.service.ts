import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Member } from '../_models/member';
import { TherapistExercises } from '../_models/TherapistExercises';

@Injectable({
  providedIn: 'root'
})
export class WorkoutService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrL;
  
  getWorkouts(): Observable<any> {
    return this.http.get<any>(this.baseUrl);
  }

  getUserWorkout(traineeLevel: string , traineeGoal: string)
  {
      return this.http.get(this.baseUrl + "users/workout/standard-workout", {
          params: {
            traineeLevel: traineeLevel,
            traineeGoal: traineeGoal
      }
    });
  }


  getHomeWorkout(fitnessLevel : string , traineeGoal : string)
  {
    return this.http.get(this.baseUrl + "users/workout/home-workout" , 
      {
        params: {
          fitnessLevel: fitnessLevel,
          traineeGoal: traineeGoal
        }
      });
  }

  getTherapyExercises() 
  {
    return this.http.get<TherapistExercises[]>(this.baseUrl + "users/workout/therapist-exercises");
  }

  

}
