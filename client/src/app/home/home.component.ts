import { Component, inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  private accountService = inject(AccountService);
  private router = inject(Router);

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');

  }
}
