import { Component, inject, OnInit } from '@angular/core';
import { ProfileService } from '../_services/profile.service';
import { Router } from '@angular/router';
import { Profile } from '../_models/profile';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-profile-list',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './profile-list.component.html',
  styleUrl: './profile-list.component.css'
})
export class ProfileListComponent implements OnInit {
  profiles: Profile[] = [];
  filteredProfiles: Profile[] = [];
  searchTerm = '';
  selectedRole = '';
  private profileService = inject(ProfileService);
  private router = inject(Router);

  ngOnInit(): void {
    this.loadProfiles();
  }

  loadProfiles(role: string = '') {
    this.profileService.getAllProfiles(role).subscribe(profiles => {
      // ✅ Exclude Admins here
      this.profiles = profiles.filter(p => p.role !== 'Admin');
      this.applyFilters();
    });
  }

  search() {
    this.applyFilters();
  }

  filterByRole(role: string) {
    this.selectedRole = role;
    this.applyFilters();
  }

  applyFilters() {
    const term = this.searchTerm.toLowerCase();
    this.filteredProfiles = this.profiles.filter(profile =>
      (!this.selectedRole || profile.role === this.selectedRole) &&
      (!this.searchTerm || profile.username.toLowerCase().includes(term))
    );
  }

  goToProfile(username: string) {
    this.router.navigate(['/profile', username]);
  }
}
