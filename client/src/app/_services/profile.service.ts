// src/app/_services/profile.service.ts
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Profile, TrainerSubscription } from '../_models/profile';


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

  addTrainerSubscription(subscription: any) {
    return this.http.post<TrainerSubscription>(this.baseUrl + 'trainersubscriptions', subscription);
  }
  
  deleteProfilePicture() {
    return this.http.delete(this.baseUrl + 'profilepicture');
  }

  addTherapistSessionPrice(sessionPrice: { title: string; price: number }) {
    return this.http.post(this.baseUrl + 'therapistsessions', sessionPrice);
  }
  deleteSubscription(subId: number) {
    return this.http.delete(this.baseUrl + 'trainersubscriptions/' + subId);
  }
  
  deleteSessionPrice(sessionId: number) {
    return this.http.delete(this.baseUrl + 'therapistsessions/' + sessionId);
  }
  
}
