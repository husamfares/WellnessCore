import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { HasRoleDirective } from '../_directives/has-role.directive';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { Profile } from '../_models/profile';
import { ProfileService } from '../_services/profile.service';
import { take } from 'rxjs';


@Component({
  selector: 'app-home',
  imports: [RouterLink, CommonModule, MatIconModule , RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  accountService = inject(AccountService);
  profileService = inject(ProfileService); 
  private router = inject(Router);
  sidebarVisible = true;
  username = this.accountService.currentUser()?.username.toString() || '';
  isSidebarClosed  = false;
  profile : Profile | null = null;
  photoUrl: string | null = null;

  ngOnInit() {
    if (this.username) {
      this.profileService.getProfile(this.username)
        .pipe(take(1))
        .subscribe({
          next: (profile) => {
            this.profile = profile;
            this.photoUrl = profile?.profilePictureUrl || null;
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
