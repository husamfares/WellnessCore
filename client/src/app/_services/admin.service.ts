import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrL;
  private http = inject(HttpClient);
  private currentUserSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  getUsersWithRoles() {
    return this.http.get<any[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles.join(','), {});
  }

  getUserRole(): string {
    const currentUser = this.currentUserSubject.value;
    return currentUser?.role || '';
  }

  isAdmin(): boolean {
    return this.getUserRole() === 'Admin';
  }

}