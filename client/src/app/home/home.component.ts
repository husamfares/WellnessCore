import { Component, inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-home',
  imports: [RouterLink,CommonModule, MatIconModule],
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
