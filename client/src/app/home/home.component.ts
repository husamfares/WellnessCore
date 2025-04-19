import { Component, inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
<<<<<<< HEAD
import { Router, RouterLink, RouterModule } from '@angular/router';
import { HasRoleDirective } from '../_directives/has-role.directive';
=======
import { Router, RouterLink } from '@angular/router';
>>>>>>> fa3a3ca2062c0bc79f9d999908112b1e3ce63688
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-home',
<<<<<<< HEAD
  imports: [RouterLink, HasRoleDirective, CommonModule, MatIconModule , RouterModule],
=======
  imports: [RouterLink,CommonModule, MatIconModule],
>>>>>>> fa3a3ca2062c0bc79f9d999908112b1e3ce63688
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  private accountService = inject(AccountService);
  private router = inject(Router);
  sidebarVisible = true;


  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');

  }

  toggleSidebar() {
    this.sidebarVisible = !this.sidebarVisible;
  }
}
