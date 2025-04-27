import { Component, inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { HasRoleDirective } from '../_directives/has-role.directive';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-home',
  imports: [RouterLink, CommonModule, MatIconModule , RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  accountService = inject(AccountService);
  private router = inject(Router);
  sidebarVisible = true;
  username = this.accountService.currentUser()?.username.toString() || '';


  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');

  }

  toggleSidebar() {
    this.sidebarVisible = !this.sidebarVisible;
  }
}
