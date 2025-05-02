// src/app/_services/profile.service.ts
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Profile } from '../_models/profile';


@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrL;

  getProfile(username: string) {
    return this.http.get<Profile>(this.baseUrl + 'profile/' + username);
  }

  updateProfile(profile: Profile) {
    return this.http.put(this.baseUrl + 'profile', profile);
  }

  getAllProfiles(role: string = '') {
    const params = role ? `?role=${role}` : '';
    return this.http.get<Profile[]>(this.baseUrl + 'profile' + params);
  }

  uploadProfilePicture(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imageUrl: string }>(
      this.baseUrl + 'profilepicture/upload',
      formData
    );
  }
}
