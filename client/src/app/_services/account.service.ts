import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = 'https://localhost:5001/api/';
  currentUser = signal<User | null>(null);
 

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
 
  register(model: any): Observable<void> {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUser.set(user);
        }
      })
    )
  }

  wellness_info(model: any)
  {
    return this.http.post<User>(this.baseUrl+ 'account/update-wellness-info' , model).pipe(
      map(user => {
        if (user) {
          this.currentUser.set(user); // Update stored user data
          
        }
      })
    );
    // what is type of return here? it is return Observable as object user
  }

  logout()
  {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}
