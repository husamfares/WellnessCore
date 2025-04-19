import { Component, inject, OnInit } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { AdminService } from '../../_services/admin.service';
import { FormsModule } from '@angular/forms';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-admin-panel',
  imports: [TabsModule,FormsModule, NgFor],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent implements OnInit{
  users: any[] = [];
  roles = ['Admin', 'Trainer', 'Therapist', 'User'];
  adminService = inject(AdminService);
  searchTerm: string = '';


  
  ngOnInit() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users;
    });
  }

  updateRoles(username: string, selectedRoles: string[]) {
    this.adminService.updateUserRoles(username, selectedRoles).subscribe({
      next: roles => {
        alert('Roles updated!');
      }
    });
  }

  filteredUsers() {
    if (!this.searchTerm) return this.users;
    return this.users.filter(user =>
      user.userName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }
}
