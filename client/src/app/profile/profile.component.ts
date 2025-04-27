import { Component, inject, OnInit } from '@angular/core';
import { ProfileService } from '../_services/profile.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Profile } from '../_models/profile';

@Component({
  selector: 'app-profile',
  imports: [FormsModule, CommonModule,ReactiveFormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  form!: FormGroup;
  isTrainer = false;
  isOwner = false;
  profile: Profile | null = null;
  private profileService = inject(ProfileService);
  private accountService = inject(AccountService); 
  private fb = inject(FormBuilder); 
  private route = inject(ActivatedRoute);
 

  ngOnInit(): void {
    const username = this.route.snapshot.paramMap.get('username')!;
    this.profileService.getProfile(username).subscribe(profile => {
      this.profile = profile;
      this.setupForm();

      const currentUsername = this.accountService.currentUser()?.username;
      this.isOwner = (currentUsername === profile.username);
    });
  }

  setupForm() {
    this.form = this.fb.group({
      mobileNumber: [this.profile?.mobileNumber, Validators.required],
      location: [this.profile?.location, Validators.required],
      gymName: [this.profile?.gymName]
    });
  }

  updateProfile() {
    if (this.form.invalid) return;

    const updatedProfile: Profile = {
      ...this.profile,
      ...this.form.value
    };
    
    this.profileService.updateProfile(updatedProfile).subscribe(() => {
      alert('Profile updated successfully!');
    });

}
}
