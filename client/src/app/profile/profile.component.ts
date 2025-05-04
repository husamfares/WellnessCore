import { Component, inject, OnInit } from '@angular/core';
import { ProfileService } from '../_services/profile.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ActivatedRoute } from '@angular/router';
import { Profile } from '../_models/profile';
import { CommonModule, NgIf } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  imports: [CommonModule, NgIf, FormsModule, MatIconModule, ReactiveFormsModule]
})
export class ProfileComponent implements OnInit {
  form!: FormGroup;
  isTrainer = false;
  isOwner = false;
  public profile: Profile | null = null;

  selectedFile?: File;
  imageUrl: string = '';

  newSubscription = {
    type: 'Online',
    title: '',
    price: 0
  };

  newSessionPrice = {
    title: '',
    price: 0
  };
  
  

  private profileService = inject(ProfileService);
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);

  isImageModalOpen = false;

  selectedText: string | null = null;

openTextModal(text: string) {
  this.selectedText = text;
}

closeTextModal() {
  this.selectedText = null;
}


  openImageModal() {
    this.isImageModalOpen = true;
  }

  closeImageModal() {
    this.isImageModalOpen = false;
  }

  ngOnInit(): void {
    const username = this.route.snapshot.paramMap.get('username')!;
    this.profileService.getProfile(username).subscribe(profile => {
      this.profile = profile;
      this.imageUrl = profile.profilePictureUrl ?? '';
      this.setupForm();
  
      const currentUsername = this.accountService.currentUser()?.username;
      this.isOwner = (currentUsername === profile.username);
  
      this.form.markAsPristine();
    });
  }

  setupForm() {
    this.form = this.fb.group({
      mobileNumber: [this.profile?.mobileNumber, Validators.required],
      location: [this.profile?.location, Validators.required],
      gymName: [this.profile?.gymName],
      clinicName: [this.profile?.clinicName]
    });
  }

  updateProfile() {
    if (this.form.invalid) return;
  
    const updatedProfile: Profile = {
      ...this.profile,
      ...this.form.value
    };
  
    this.profileService.updateProfile(updatedProfile).subscribe(() => {
      // ✅ Reload the updated profile data
      this.refreshProfile();
    });
  }

  triggerFileSelect() {
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.click();
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.selectedFile = input.files[0];

    this.uploadImage();
  }

  uploadImage() {
    if (this.selectedFile) {
      this.profileService.uploadProfilePicture(this.selectedFile).subscribe(res => {
        this.imageUrl = res.imageUrl;
        if (this.profile) {
          this.profile.profilePictureUrl = res.imageUrl;
        }
      });
    }
  }
  deleteProfilePicture() {
    this.profileService.deleteProfilePicture().subscribe(() => {
      this.imageUrl = '';
      if (this.profile) {
        this.profile.profilePictureUrl = '';
      }
    });
  }
  

  addSubscription() {
    if (!this.newSubscription.title || this.newSubscription.price <= 0) {
      alert('Please enter a valid title and price.');
      return;
    }
  
    this.profileService.addTrainerSubscription(this.newSubscription).subscribe(sub => {
      if (this.profile) {
        this.profile.subscriptions.push(sub);  
        this.newSubscription = { type: 'Online', title: '', price: 0 };
      }
    });
  }
  
  refreshProfile() {
    const username = this.route.snapshot.paramMap.get('username')!;
    this.profileService.getProfile(username).subscribe(profile => {
      this.profile = profile;
      this.imageUrl = profile.profilePictureUrl ?? '';
      this.setupForm();
      this.form.markAsPristine();  // ✅ Reset form status
    });
  }

  addSessionPrice() {
    if (!this.newSessionPrice.title || this.newSessionPrice.price <= 0) {
      alert('Please enter a valid title and price.');
      return;
    }
  
    this.profileService.addTherapistSessionPrice(this.newSessionPrice).subscribe(() => {
      // Reload the profile to show new session price
      this.refreshProfile();
      // Reset form
      this.newSessionPrice = { title: '', price: 0 };
    });
  }

  deleteSubscription(subId: number) {
    if (confirm('Are you sure you want to delete this subscription?')) {
      this.profileService.deleteSubscription(subId).subscribe(() => {
        this.profile!.subscriptions = this.profile!.subscriptions!.filter(s => s.id !== subId);
      });
    }
  }
  
  deleteSessionPrice(sessionId: number) {
    if (confirm('Are you sure you want to delete this session price?')) {
      this.profileService.deleteSessionPrice(sessionId).subscribe(() => {
        this.profile!.sessionPrices = this.profile!.sessionPrices!.filter(s => s.id !== sessionId);
      });
    }
  }
  
  
}
