import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrL;
  currentUser = signal<User | null>(null);
  roles = computed(() => {
  
    const user = this.currentUser() 
    if(user && user.token){
      const role = JSON.parse(atob(user.token.split('.')[1])).role
      return Array.isArray(role) ? role : [role];
  }
    return [];
  })

  isAdmin = computed(() => this.roles().includes('Admin'));
  
  isTrainerOrTherapist = (): boolean => {
    const userRoles = this.roles();
    return userRoles.includes('Trainer') || userRoles.includes('Therapist');
  }
 
  login(model: any)
  {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUser.set(user);
        }
      })
    )
  }
 
  register(model: any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUser.set(user);
        }
      })
    )
  }


  logout()
  {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

 
}
