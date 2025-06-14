import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Profile } from '../_models/profile';
import { ProfileService } from '../_services/profile.service';
import { take } from 'rxjs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@Component({
  selector: 'app-home',
  imports: [RouterLink, CommonModule, MatIconModule , RouterModule, MatTooltipModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  accountService = inject(AccountService);
  profileService = inject(ProfileService); 
  private router = inject(Router);
  username = this.accountService.currentUser()?.username.toString() || '';
  isSidebarClosed  = true;
  profile : Profile | null = null;
  photoUrl: string | null = null;
  userId: number | null = null;

  ngOnInit() {
    if (this.username) {
      this.profileService.getProfile(this.username)
        .pipe(take(1))
        .subscribe({
          next: (profile) => {
            this.profile = profile;
            this.photoUrl = profile?.profilePictureUrl || null;
            this.userId = profile.id; 
          },
          error: (err) => {
            console.error('Failed to load profile', err);
          }
        });
    }
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');

  }

toggleSidebar() {
  this.isSidebarClosed  = !this.isSidebarClosed ;
}

}
