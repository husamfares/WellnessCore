import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { fitnessCheckInfo } from '../_models/fitnessCheckInfo';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrL;

  getUserFitnessInfo(): Observable<Member> {
    return this.http.get<Member>(this.baseUrl+'users/user/fitness-check'); // Adjust URL as needed
  }

  setFitnessLevel(fitnessLevel : string , traineeGoal : string)
  {
    console.log("in user service : fitnessLevel" , fitnessLevel , traineeGoal);
    return this.http.put(this.baseUrl+ 'users/user/update-fitness-value' , {
      fitnessLevel : fitnessLevel,
      goal : traineeGoal
    });
  }

}
